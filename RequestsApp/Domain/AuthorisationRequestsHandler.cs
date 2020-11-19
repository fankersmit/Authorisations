using  System;

namespace RequestsApp.Domain
{
    public class AuthorisationRequestsHandler
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
            return $"{message}:{_random.Next(0,40000)}";
        }
    }
}