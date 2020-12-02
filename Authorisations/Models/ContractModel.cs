using System;
using System.Collections.Generic;

namespace Authorisations.Models
{
    public class ContractModel
    {
        public Guid ContractID { get; set; }
        public OrganisationModel Organisation { get; set;  }
        public  string AuthorizerMailAddress { get; set; }
        public DateTime StartDate { get; set;  }
        public DateTime EndDate { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
    }
}