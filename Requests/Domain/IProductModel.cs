using System;

namespace Requests.Domain
{
    public interface IProductModel
    {
        string Name { get; set; }
        string Description { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }
}