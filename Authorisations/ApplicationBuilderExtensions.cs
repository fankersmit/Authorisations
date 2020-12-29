using Authorisations.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Authorisations
{
    public static class ApplicationBuilderExtensions
    {
        //store a single long-living object
        private static RabbitMqDefaultClient DefaultClient { get; set; }

        public static IApplicationBuilder UseRabbitDefaultClient(this IApplicationBuilder app)
        {
            DefaultClient = app.ApplicationServices.GetService<RabbitMqDefaultClient>();

            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            lifetime.ApplicationStarted.Register(OnDefaultStarted);

            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            lifetime.ApplicationStopping.Register(OnDefaultStopping);

            return app;
        }

        //store a single long-living object
        private static RabbitMqRpcClient RpcClient { get; set; }

        public static IApplicationBuilder UseRabbitRpcClient(this IApplicationBuilder app)
        {
            RpcClient = app.ApplicationServices.GetService<RabbitMqRpcClient>();

            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            lifetime.ApplicationStarted.Register(OnRpcStarted);

            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            lifetime.ApplicationStopping.Register(OnRpcStopping);

            return app;
        }

        private static void OnRpcStarted()
        {
            RpcClient.Register();
        }

        private static void OnRpcStopping()
        {
            RpcClient.Deregister();
        }

        private static void OnDefaultStarted()
        {
            DefaultClient.Register();
        }

        private static void OnDefaultStopping()
        {
            DefaultClient.Deregister();
        }
    }
}
