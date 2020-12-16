using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Requests.Domain
{
    public class Person : IReconstitutable<Person>
    {
        // properties
        public int ID { get; private set; }
        public string FirstName { get; private set;}
        public string LastName { get; private set;}
        public string Salutation { get; private set;}

        // ctors
        public Person(int  Id, string firstName, string lastName, string salutation = "")
        {
            ID = Id;
            FirstName = firstName;
            LastName = lastName;
            Salutation = salutation;
        }
        
        public Person(string firstName, string lastName, string salutation = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Salutation = salutation;
        }

        private Person()
        {
        }

        // methods
        public  string FullName()
        {
            var fullName =  $"{Salutation} {FirstName} {LastName}";
            return fullName.Trim().Replace("  ", " ");
        }

        public bool Reconstitute(JsonElement personElement)
        {
            var missingProperties = 0;
            PropertyInfo[] properties = typeof(Person).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Type t = property.PropertyType;
                switch (t.Name)
                {
                    case "String":
                        string stringValue = string.Empty;
                        try
                        {
                            stringValue = personElement.GetProperty(property.Name).GetString();
                            property.SetValue(this, stringValue);
                        }
                        catch (KeyNotFoundException e)
                        {
                            ++missingProperties;
                        }

                        break;
                    case "Int32":
                        var intValue = 0;
                        try
                        {
                            intValue = personElement.GetProperty(property.Name).GetInt32();
                            property.SetValue(this, intValue);
                        }
                        catch (KeyNotFoundException e)
                        {
                            ++missingProperties;
                        }
                        break;
                }
            }
            return (missingProperties == 0);
        }
    }
}