using System;

namespace Tests.Helpers
{
    public readonly struct OrganisationData
    {
        public readonly Guid ID;
        public readonly string Name;
        public readonly string Description;
        public readonly string DomainName;

        public OrganisationData(string Id, string name, string description, string domainName)
        {
            Name = name;
            Description = description;
            ID = Guid.Parse(Id);
            DomainName = domainName;
        }
    }
}