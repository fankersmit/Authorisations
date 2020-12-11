using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using RequestsApp.Infrastructure;


namespace RequestsApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            // setup, does all configuring
            var startup = new Startup();

            // create the database if needed
            var  requestContext = startup.Provider.GetService<RequestDbContext>();
            requestContext.Database.EnsureCreated();

            var service = startup.Provider.GetService<RabbitMQServer>();
            service.Run(requestContext);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            service.Stop();
        }

    }
}