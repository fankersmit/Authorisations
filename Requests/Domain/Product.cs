using System;

namespace Requests.Domain
{
    public class Product : IProductModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Product(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}