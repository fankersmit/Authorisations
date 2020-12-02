using System;
using System.Reflection.PortableExecutable;
using Requests.Domain;

namespace Authorisations.Models
{
    public class RequestModel
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public RequestStatus  Status { get; set; }
        public string Remarks { get; set; }
        public ContractModel Contract { get; set; }
        public PersonModel Applicant { get; set; }
    }
}