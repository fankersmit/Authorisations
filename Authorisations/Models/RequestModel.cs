using System;
using Requests.Shared.Domain;

namespace Authorisations.Models
{
    public class RequestModel : ICommand
    {
        // properties
        public Guid ID { get; set; }
        public int Version { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public RequestStatus  Status { get; set; }
        public string Remarks { get; set; }
        public ContractModel Contract { get; set; }
        public PersonModel Applicant { get; set; }
        public Commands Command { get; set; }
        
        // ctor
        public RequestModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            DateCreated = DateTime.UtcNow;
            DateLastUpdated = DateCreated;
            ID = Guid.NewGuid();
            Version = 1;
            Status = RequestStatus.New;
            Remarks = "";
            Command = Commands.NoOp;
        }
    }
}