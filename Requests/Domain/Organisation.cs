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
            Name = name;
            Description = description;
        }
    }
}