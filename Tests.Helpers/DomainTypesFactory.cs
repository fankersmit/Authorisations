﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Requests.Domain;

namespace Tests.Helpers
{
    public sealed class DomainTypesFactory
    {
        private static readonly DomainTypesFactory _instance = new DomainTypesFactory();
        
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DomainTypesFactory()
        {
        }

        private DomainTypesFactory()
        {
            _random = new Random( DateTime.Now.Minute);  
        }
        
        public static DomainTypesFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        private readonly Random _random; 
        
        
        #region testdata
        private static List<string> firstNames = new List<string> {
            "Ayana", "Tracie", "Shiela", "Duncan", "Tonisha", 
            "Daren", "Dong", "Marian", "Mad", "Alica", "Joke"
        };

        private static List<string> lastNames = new List<string>  {
            "Withrow", "Mastroianni", "Rushin", "Edmundson", "Mcneese", "Mcneese",
            "Shippy", "Mo Osei", "de Schank", "van Nolte", "Strasser"
        };
        
        private static IList<OrganisationData>  Orgs = new ReadOnlyCollection<OrganisationData> 
        ( new[]{
            new OrganisationData("f9782b64-a7f5-46f1-84ec-d58e5e0de030", "Hoek & Klein Huisartsen","Voor al uw geestelijke en lichamelijke pijntjes", "hoekklein.nl"),
            new OrganisationData("10b79e1f-60b4-48a8-8697-08349a16dea7", "Fysio Veen, Visser en Smits", "specialist in in rug en knieklachten", "fysioveenvisser.nl"),
            new OrganisationData("89f35353-18d1-4ba6-a21e-223bbc8918e2",  "Koning en Heuvel Zorgspecialisten", "Onvergetelijke thuiszorg", "khzorgspecialisten.com"),
            new OrganisationData("2af52cfe-bedb-46d3-a5f2-9d635f57c3e5", "GGZ Centaal", "Ambulante GGZ in amersfoort en omgeving", "ggzcentraal.eu"),
            new OrganisationData("09c91eb1-8e72-454c-95e3-5cae3591fd5b", "Dekker Medisupport", "Hulpmiddelen, huur en verkoop", "medisupport.com"),
            new OrganisationData("4f5ef31f-0259-4477-a558-d401e227aa03", "Linden", "Tandartsen", "lindentandartsen.nl"),
            new OrganisationData("71b17501-7d51-4927-be73-3e561c87a190", "Ruiter, Vries-Ruiter", "Orthopedie in elke maat", "ruiterorthopedie.nl"),
            new OrganisationData("a63f1894-641d-48e4-8871-2651a8c17013", "Haan ICT", "Excellentezorg administratie voor elke sector", "haanict.com"),
            new OrganisationData("6021cffe-5a50-4726-9656-fc30b13d0220", "ZorgMij", "", "zorgmij.nl" )
        }); 
        #endregion

        public Person CreateApplicant()
        {
            var idx1 = _random.Next(0, 10);
            var idx2 = _random.Next(0, 10);
            return new Person(firstNames[idx1], lastNames[idx2]); 
        }

        public Organisation CreateOrganisation()
        {
            var idx = _random.Next(0, 9);
            var org = Orgs[idx];
            return  new Organisation( org.ID,org.Name, org.Description);
        }
        
        public Organisation CreateOrganisation( OrganisationData org)
        {
            return  new Organisation( org.ID,org.Name, org.Description);
        }

        public Contract CreateContract(OrganisationData orgData )
        {
            var duration = _random.Next(0, 5);
            var idx = _random.Next(0, 10); 
            var email = CreateEmail( firstNames[idx], lastNames[idx], orgData.DomainName  );
            var organisation = CreateOrganisation(orgData);
            var startDate = DateTime.Now.Subtract(new TimeSpan(48, 0, 0));
            var endDate = DateTime.Now.AddYears(duration);
            return new Contract(organisation, email, startDate, endDate);           
        }
   
        public Contract CreateContract( Organisation organisation )
        {
            var duration = _random.Next(0, 5);
            var idx = _random.Next(0, 10); 
            var email = CreateEmail( firstNames[idx], lastNames[idx], "gmail.com");
            var startDate = DateTime.Now.Subtract(new TimeSpan(48, 0, 0));
            var endDate = DateTime.Now.AddYears(duration);
            return new Contract(organisation, email, startDate, endDate);           
        }

        public Product CreateProduct()
        {
            return new Product("AGB", "Algemeen gegevens beheer");    
        }
        
        public AccountRequest CreateAccountRequest( Person applicant, Contract contract )
        {
            return new AccountRequest(applicant, contract);
        }
        
        public AccountRequest CreateAccountRequest()
        {
            var applicant = CreateApplicant();
            var org = CreateOrganisation();
            var contract = CreateContract(org);
            return new AccountRequest(applicant, contract);
        }
        
        // helper methods
        private MailAddress CreateEmail(string firstName, string lastName, string domainName)
        {
           var email = $"{firstName}.{lastName}@{domainName}".Trim().Trim('.');
           return new MailAddress(email);
        }
    }

    public readonly struct OrganisationData
    {
        public readonly Guid ID;
        public readonly string Name;
        public readonly string Description;
        public readonly string DomainName;

        public OrganisationData( string Id, string name, string description , string domainName)
        {
            Name = name;
            Description = description;
            ID = Guid.Parse(Id);
            DomainName = domainName;
        }
    }
}