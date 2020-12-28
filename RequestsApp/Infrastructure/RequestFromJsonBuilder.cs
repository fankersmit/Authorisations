using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Infrastructure
{
    public class RequestFromJsonBuilder
    {
        // fields  
        private readonly ILogger<RequestFromJsonBuilder> _logger;
       
        // properties

        // ctors
        public RequestFromJsonBuilder( ILogger<RequestFromJsonBuilder> logger)
        {
            _logger = logger;
        }

        // methods
        public AccountRequest GetRequest(JsonElement jsonRequest)
        {
            var request = new AccountRequest();
           
            PropertyInfo[] properties = typeof(AccountRequest).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Type t = property.PropertyType;
                switch (t.Name)
                {
                    // Status
                    case "RequestStatus":
                        string enumValue = jsonRequest.GetProperty(property.Name).GetString();
                        property.SetValue(request, Enum.Parse<RequestStatus>(enumValue));
                        break;
                    // Person
                    case "Person":
                        var applicantValue = GetApplicant(jsonRequest.GetProperty("Applicant"));
                        property.SetValue(request , applicantValue);
                        break;
                    // Contract
                    case "Contract":
                        var contractValue = GetContract(jsonRequest.GetProperty("Contract"));
                        property.SetValue(request, contractValue);
                        break;
                    // DateCreated, DateLastUpdated
                    case "DateTime":
                        var  dateTimeVal = jsonRequest.GetProperty(property.Name).GetDateTime();
                        property.SetValue(request , dateTimeVal);
                        break;
                    // Remarks
                    case "String":
                        var strVal = jsonRequest.GetProperty(property.Name).GetString();
                        property.SetValue(request , strVal);
                        break;
                    // ID
                    case "Guid":
                        var guidVal = jsonRequest.GetProperty(property.Name).GetGuid();
                        property.SetValue(request , guidVal);
                        break;
                    // Version
                    case "Int32":
                        var intVal = jsonRequest.GetProperty(property.Name).GetInt32();
                        property.SetValue(request , intVal);
                        break;
                }
            }
            return request;
        }

        public Person GetApplicant(JsonElement applicantElement )
        {
            var applicant = new Person();
            
            PropertyInfo[] properties = typeof(Person).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Type t = property.PropertyType;
                switch (t.Name)
                {
                    // FirstName, LastName, Salutation
                    case "String":
                        var strVal = applicantElement.GetProperty(property.Name).GetString();
                        property.SetValue(applicant , strVal);
                        break;
                    
                    // ID
                    case "Int32":
                        var intVal = applicantElement.GetProperty(property.Name).GetInt32();
                        property.SetValue(applicant , intVal);
                        break;
                }
            }
            return applicant;
        }

        public IList<Product> GetProducts(JsonElement productsElement )
        {
            if (productsElement.ValueKind != JsonValueKind.Array)
            {
                var message = ($"Argument not a list of products: : {productsElement.GetRawText().Substring(15)}...") ;
                throw new ArgumentException(message);
            }
            
            var products = new List<Product>();
            foreach (JsonElement productElement in productsElement.EnumerateArray())
            {
                var product = new Product();
                product.ID = productElement.GetProperty("ID").GetInt32();
                product.Name = productElement.GetProperty("Name").GetString();
                product.Description = productElement.GetProperty("Description").GetString();
                product.StartDate = productElement.GetProperty("StartDate").GetDateTime();
                product.EndDate = productElement.GetProperty("EndDate").GetDateTime();
                products.Add(product);
            }
            return products;
        }

        public Contract GetContract( JsonElement jsonElement)
        {
            var contract = new Contract();
            
            PropertyInfo[] properties = typeof(Contract).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                switch (property.Name)
                {
                    // Products
                    case "Products":
                        IList<Product> prodsVal = GetProducts(jsonElement.GetProperty("Products")); 
                        property.SetValue(contract , prodsVal);
                        break;                    
                    // Organisation
                    case "Organisation":
                        Organisation orgVal = GetOrganisation(jsonElement.GetProperty("Organisation")); 
                        property.SetValue(contract , orgVal);
                        break;
                    // CReation and update Date
                    case "StartDate":
                    case "EndDate":
                        var  guidVal = jsonElement.GetProperty(property.Name).GetDateTime();
                        property.SetValue(contract , guidVal);
                        break;
                    // Email
                    case "AuthorizerMailAddress":
                        var strVal = jsonElement.GetProperty(property.Name).GetString();
                        property.SetValue(contract , strVal);
                        break;
                    // ID
                    case "ID":
                        var intVal = jsonElement.GetProperty(property.Name).GetGuid();
                        property.SetValue(contract , intVal);
                        break;
                }
            }
            return contract;
        }

        public Organisation GetOrganisation(JsonElement jsonElement)
        {
            var organisation = new Organisation();
            
            PropertyInfo[] properties = typeof(Organisation).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Type t = property.PropertyType;
                switch (t.Name)
                {
                    // Name, Description
                    case "String":
                        var strVal = jsonElement.GetProperty(property.Name).GetString();
                        property.SetValue(organisation , strVal);
                        break;
                    
                    // ID
                    case "Guid":
                        var guidVal = jsonElement.GetProperty(property.Name).GetGuid();
                        property.SetValue(organisation , guidVal);
                        break;
                }
            }
            return organisation;
        }

        public Commands GetCommand(JsonElement jsonElement)
        {
            string enumValue = jsonElement.GetString();
            return Enum.Parse<Commands>(enumValue);
        }
    }
}
