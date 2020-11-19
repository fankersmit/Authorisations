using System;
using System.Collections.Generic;

namespace Requests.Domain
{
    public interface IContractModel
    {
        Guid ContractID { get; }
        Organisation Organisation { get; }
        string Email_AuthorisationManager { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        IList<Product> Products { get; }
    }
}