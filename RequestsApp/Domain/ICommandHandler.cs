using System;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public interface ICommandHandler
    {
        bool Handle(RequestBase request, Commands command);
        event EventHandler<CommandHandledEventArgs> CommandHandled;
    }
}