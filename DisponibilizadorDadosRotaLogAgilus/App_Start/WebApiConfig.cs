using System.Net.Http.Headers;
using System.Web.Http;

namespace DisponibilizadorDadosRotaLogAgilus
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Json Returns by Default
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
