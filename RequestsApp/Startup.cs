using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using RequestsApp.Infrastructure;

namespace RequestsApp
{
    public class Startup
    {
        private readonly IConfiguration  _configuration;
        private readonly IServiceProvider  _provider;
        
        public IServiceProvider Provider => _provider;
        public IConfiguration Configuration => _configuration;

        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            var services = new ServiceCollection();
            ConfigureServices( services );
            _provider = services.BuildServiceProvider();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString  = GetConnectionString();
            
            services.AddLogging(configure => configure.AddConsole())
                .AddTransient<RabbitMQServer>()
                .AddTransient<ICommandHandler, CommandHandler>()
                .AddTransient<IQueryHandler, QueryHandler>()
                .AddEntityFrameworkSqlite()
                .AddDbContext<RequestDbContext>(
       (serviceProvider, options) =>
                        options.UseSqlite(connectionString)
                               .UseInternalServiceProvider(serviceProvider));
        }

        private string  GetConnectionString()
        {
            return Configuration.GetConnectionString("DefaultConnection");
        }
    }
}