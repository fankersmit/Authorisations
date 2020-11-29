using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestsApp.Domain;
using RequestsApp.Infrastructure;

namespace RequestsApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                .AddTransient<RabbitMQServer>()
                .AddTransient<AuthorisationRequestsHandler>()
                .AddEntityFrameworkSqlite().AddDbContext<RequestContext>();
            
            // make sure there is a database
            using(var client = new RequestContext())
            {
                client.Database.EnsureCreated();
            }
        }
    }
}