using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using RequestsApp.Domain;
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
            using( var requestContext = startup.Provider.GetRequiredService<RequestDbContext>())
            {
                requestContext.Database.EnsureCreated();
            }

            try
            {
                var service = startup.Provider.GetRequiredService<RabbitMQServer>();
                ICommandHandler commandHandler = startup.Provider.GetRequiredService<ICommandHandler>();
                IQueryHandler queryHandler = startup.Provider.GetRequiredService<IQueryHandler>();
                // wire up event handling
                var queryContext = startup.Provider.GetRequiredService<RequestDbContext>();
                commandHandler.CommandHandled += queryContext.OnCommandExecuted;
                service.Run(commandHandler, queryHandler);
            }
            catch ( InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

    }
}
