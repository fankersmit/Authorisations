using System;
using Requests.Shared.Domain;

namespace Requests.Domain
{
    public class CommandHandledEventArgs : EventArgs
    {
        public Commands CommandHandled { get; set; }
        public RequestBase Request { get; set; }
    }
}