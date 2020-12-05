using System;
using Requests.Shared.Domain;

namespace Requests.Domain
{
    public interface ICommandHandler
    {
        bool Handle(RequestBase request, Commands command);
        event EventHandler<CommandHandledEventArgs> CommandHandled;
    }
}