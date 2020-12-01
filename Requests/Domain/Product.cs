using System;

namespace Requests.Domain
{
    public class Product : IProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        private Product()
        {
        }
        
        public Product(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}