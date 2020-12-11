using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Requests.Domain
{
    public class Contract 
    {
        public Guid ID  { get; private set; }
        
        public Organisation Organisation { get; private set; }
        public string AuthorizerMailAddress { get; private set; }
        
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public  ICollection<Product> Products { get; private set; } 

        private Contract()
        {
            ID = Guid.NewGuid();
            Products = new List<Product>();
            AddDefaultProducts();
        }
        
        public Contract(Organisation org, string email, DateTime start, DateTime end) : this()
        {
            Organisation = org;
            AuthorizerMailAddress = email;
            StartDate = start;
            EndDate = end;
            
        }

        private void AddDefaultProducts()
        {
            var name ="MijnVektis";
            var description = "Self service van mijn vektis account";
            var mijnVektisProduct = new Product(name, description);
            mijnVektisProduct.StartDate = DateTime.Today.Subtract(new TimeSpan(24,0,0));
            mijnVektisProduct.EndDate = DateTime.Today.AddYears(3);
            Products.Add( mijnVektisProduct);
            
            name ="MijnAGB";
            description = "Self service van mijn persoonlijjke AGB code";
            var mijnAGBProduct = new Product(name, description);
            mijnAGBProduct.StartDate = DateTime.Today.Subtract(new TimeSpan(24,0,0));
            mijnAGBProduct.EndDate = DateTime.Today.AddYears(3);
            Products.Add( mijnAGBProduct);
        }
    }
}