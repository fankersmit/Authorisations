using System;
using System.Runtime.InteropServices;

namespace Requests.Domain
{
    public class Organisation : IOrganisationModel
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Organisation(string name, string description)
        {
            Id  = Guid.NewGuid();
            Initialize(name, description);
        }
        
        public Organisation(Guid ID, string name, string description)
        {
            Id = ID;
            Initialize(name, description);
        }

        private void Initialize(string name, string description)
        {
            Name = name;
            Description = description;            
        }
    }
}