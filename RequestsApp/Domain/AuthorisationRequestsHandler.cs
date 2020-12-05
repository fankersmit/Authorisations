using  System;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public class AuthorisationRequestsHandler : ICommandHandler
    {
        private readonly Random _random; 
      
        // ctor
        public AuthorisationRequestsHandler()
        {
            // phony return value creation
            _random = new Random( DateTime.Now.Minute);            
        }
        
        public string ResponseFor(string message)
        {
            return  $"{message}:{_random.Next(0,40000)}";    
        }
        
        public string ProcessMessage(string message)
        {
            Console.WriteLine(message);
            return string.Empty;
        }

        public bool Handle(RequestBase request, Commands command)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<CommandHandledEventArgs> CommandHandled;
    }
}