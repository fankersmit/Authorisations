using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Requests.Domain
{
    public interface IContractModel
    {
        Guid ContractID { get; }
        Organisation Organisation { get; }
        MailAddress AuthorizerMailAddress { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        IList<Product> Products { get; }
    }
}