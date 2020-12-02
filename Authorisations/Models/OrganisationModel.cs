using System;

namespace Authorisations.Models
{
    public class OrganisationModel
    {
        public Guid OrganisationId { get; set;  }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}