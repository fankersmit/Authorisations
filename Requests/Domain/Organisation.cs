using System;
using System.Runtime.InteropServices;

namespace Requests.Domain
{
    public class Organisation
    {
        // properties
        public Guid ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        // ctors
        public Organisation(string name, string description)
        {
            ID  = Guid.NewGuid();
            Initialize(name, description);
        }
        
        public Organisation(Guid ID, string name, string description)
        {
            this.ID = ID;
            Initialize(name, description);
        }

        private Organisation()
        {
        }

        private void Initialize(string name, string description)
        {
            Name = name;
            Description = description;            
        }
    }
}