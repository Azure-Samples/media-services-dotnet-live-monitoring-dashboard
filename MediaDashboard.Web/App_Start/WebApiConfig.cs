using System.Web.Http;

namespace MediaDashboard.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           
            // Web API routes
            config.MapHttpAttributeRoutes();
            
            // Default catch-all
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ChildApi",
                routeTemplate: "api/Accounts/{account}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "Action",
                routeTemplate: "api/Accounts/{account}/{controller}/{id}/{action}");
        }
    }
}
