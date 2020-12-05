using Requests.Shared;
using Requests.Shared.Domain;

namespace Authorisations.Models
{
    interface ICommand
    {
        public Commands Command { get; set; }
    }
}