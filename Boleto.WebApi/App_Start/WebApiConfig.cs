using System;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Boleto.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Bradesco",
                    action = "Index",
                    id = RouteParameter.Optional
                }
            );

            config.EnableSystemDiagnosticsTracing();

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings
                .Add(new System.Net.Http.Formatting.RequestHeaderMapping("Accept",
                              "text/html",
                              StringComparison.InvariantCultureIgnoreCase,
                              true,
                              "application/json"));
        }
    }
}
