using System;
using Requests.Shared.Domain;

namespace Requests.Domain
{
    public class CommandHandler : ICommandHandler
    {
        // ctors
        public CommandHandler()
        {
        }

        // methods
        public bool Handle(RequestBase request, Commands command)
        {
            bool retval = false;

            switch (command)
            {
                case Commands.Submit:
                    retval = request.Submit();
                    // inform interested parties
                    RaiseCommandHandledEvent( request, command);
                    break;

                default:
                    break;
            }
            return retval;
        }

        private void RaiseCommandHandledEvent( RequestBase request, Commands command)
        {
            var eventHandler = this.CommandHandled;
            if (eventHandler != null)
            {
                var eventArgs = new CommandHandledEventArgs()
                {
                    CommandHandled = command,
                    Request = request
                };
                // raise event
                eventHandler(this, eventArgs);
            }
        }

        // events
        public event EventHandler<CommandHandledEventArgs> CommandHandled;
    }
}