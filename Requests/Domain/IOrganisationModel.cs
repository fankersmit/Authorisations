using System;

namespace Requests.Domain
{
    public interface IOrganisationModel
    {
        Guid Id { get; }
        string Description { get; }
    }
}