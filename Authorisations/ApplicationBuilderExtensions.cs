using Authorisations.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Authorisations
{
    public static class ApplicationBuilderExtensions
    {
        //store a single long-living object
        private static RabbitMqClient Client { get; set; }

        public static IApplicationBuilder UseRabbitRpcClient(this IApplicationBuilder app)
        {
            Client = app.ApplicationServices.GetService<RabbitMqClient>();

            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            lifetime.ApplicationStarted.Register(OnStarted);

            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            lifetime.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStarted()
        {
            Client.Register();
        }

        private static void OnStopping()
        {
            Client.Deregister();    
        }
    }
}