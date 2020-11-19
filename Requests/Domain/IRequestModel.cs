using System;
using System.Reflection.PortableExecutable;
using Requests.Domain;

namespace Requests.Domain
{
    public interface IRequestModel
    {
        Guid Id { get;  }
        DateTime DateCreated { get;  }
        DateTime DateLastUpdated { get; }
        RequestStatus  Status { get;  }
        string Remarks { get;  }
        
        Contract Contract { get;  }
        Person Applicant { get; }
    }
}