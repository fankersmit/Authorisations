using System;
using Microsoft.Extensions.DependencyInjection;
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
            
            // make sure there is a database
            using(var client = new RequestContext())
            {
                client.Database.EnsureCreated();
            }

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var requestHandler = new AuthorisationRequestsHandler();
                var app = serviceProvider.GetService<RabbitMQServer>();
                app.Run();

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure =>configure.AddConsole())
                .AddTransient<RabbitMQServer>()
                .AddTransient<AuthorisationRequestsHandler>()
                .AddEntityFrameworkSqlite().AddDbContext<RequestContext>();
        }
    }
 
}