using System.Web.Http;

namespace MediaDashboard.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "StatusApi",
                routeTemplate: "api/Status/AmsAccount/{amsAccountId}/Pipeline/{channelId}",
                defaults: new { amsAccountId = RouteParameter.Optional, channelId = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
             name: "MetricMapRoute",
             routeTemplate: "api/Metric/AmsAccount/{amsAccountId}/Channel/{channelId}",
             defaults: new {
                 amsAccountId = RouteParameter.Optional,
                 channelId = RouteParameter.Optional
             }
         );
        }
    }
}
