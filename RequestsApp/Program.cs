using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RequestsApp.Domain;
using RequestsApp.Infrastructure;
    

namespace RequestsApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            // setup
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                AuthorisationRequestsHandler requestHandler = new AuthorisationRequestsHandler();
                RabbitMQServer app = serviceProvider.GetService<RabbitMQServer>();
                app.Run(requestHandler);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure =>configure.AddConsole())
                .AddTransient<RabbitMQServer>();
        }
    }
}