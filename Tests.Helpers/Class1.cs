using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Requests.Domain;

namespace Tests.Helpers
{
    public static class DomainTypesFactory
    {
        private static Random _random = new Random( DateTime.Now.Minute);  
        
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
            new OrganisationData("6021cffe-5a50-4726-9656-fc30b13d0220", "ZorgMij" ),
            new OrganisationData("f9782b64-a7f5-46f1-84ec-d58e5e0de030", "Hoek & Klein Huisartsen","Voor al uw geestelijke en lichamelijke pijntjes"),
            new OrganisationData("10b79e1f-60b4-48a8-8697-08349a16dea7", "Fysio Veen, Visser en Smits"),
            new OrganisationData("89f35353-18d1-4ba6-a21e-223bbc8918e2",  "Koning en Heuvel Zorgspecialisten"),
            new OrganisationData("2af52cfe-bedb-46d3-a5f2-9d635f57c3e5", "GGZ Centaal"),
            new OrganisationData("09c91eb1-8e72-454c-95e3-5cae3591fd5b", "Dekker Medisupport"),
            new OrganisationData("4f5ef31f-0259-4477-a558-d401e227aa03", "Linden "),
            new OrganisationData("71b17501-7d51-4927-be73-3e561c87a190", "Ruiter, Vries-Ruiter"),
            new OrganisationData("a63f1894-641d-48e4-8871-2651a8c17013", "Haan ICT")
        }); 
            
        #endregion

        public static Person CreateApplicant()
        {
            var idx1 = _random.Next(0, 10);
            var idx2 = _random.Next(0, 10);
            return new Person(firstNames[idx1], lastNames[idx2]); 
        }

        public static Organisation CreateOrganisation()
        {
            var idx = _random.Next(0, 9);
            var org = Orgs[idx];
            return  new Organisation( org.ID,org.Name, org.Description);
        }

        public static Contract CreateContract( Organisation organisation )
        {
            var duration = _random.Next(0, 5);
            var email = "joke.deGraaf@zorgmij.eu";
            var startDate = DateTime.Now.Subtract(new TimeSpan(48, 0, 0));
            var endDate = DateTime.Now.AddYears(duration);
            return new Contract(organisation, email, startDate, endDate);           
        }

        public static Product CreateProduct()
        {
            return new Product("AGB", "Algemeen gegevens beheer");    
        }

        static string CreateEmail(string PartOne, string PartTo)
        {
            // com nl eu 
            // create extamsion list
            // create email $"{firstPart}.{secondPart}@{OrgName}.{extension}
            return ""; //adres
        }
    }

    public readonly struct OrganisationData
    {
        public readonly Guid ID;
        public readonly string Name;
        public readonly string Description;

        public OrganisationData( string Id, string name, string description = "")
        {
            Name = name;
            Description = description;
            ID = Guid.Parse(Id);
        }
    }



}