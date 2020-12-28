using System;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public class CommandHandler : ICommandHandler
    {
        // events
        public event EventHandler<CommandHandledEventArgs> CommandHandled;

        // ctors
        public CommandHandler( )
        {
        }

        // methods
        public bool Handle(RequestBase request, Commands command)
        {
            bool retval = false;

            switch (command)
            {
                case Commands.NoOp:
                    break;
                case Commands.Submit:
                    retval = request.Submit();
                    break;
                case Commands.Cancel:
                    retval = request.Cancel();
                    break;
                case Commands.Confirm:
                    retval = request.Confirm();
                    break;                    
                case Commands.Approve:
                    retval = request.Approve();
                    break;
                case Commands.Disapprove:
                    retval = request.Disapprove();
                    break;
                case Commands.Conclude:
                    retval = request.Conclude();
                    break;
                case Commands.Remove:
                    retval = request.Remove();
                    break;
            }
            
            if (retval == true)
            {
                // inform interested parties like database                        
                RaiseCommandHandledEvent( request, command);
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
    }
}
