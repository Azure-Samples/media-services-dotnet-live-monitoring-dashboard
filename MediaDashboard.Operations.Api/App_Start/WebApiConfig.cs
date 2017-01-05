using System.Linq;
using System.Web.Http;

namespace MediaDashboard.Operations.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable CORS so it works in a real deployment.
            config.EnableCors();
           
            // Web API routes
            config.MapHttpAttributeRoutes();

            ////the accountName is used here as a visual cue to ensure the proper
            ////WAMS deployment name is being used
            //config.Routes.MapHttpRoute(
            //    name: "OperationsApi",
            //    routeTemplate: "api/Operations/MediaServicesAccount/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "ProgramsApi",
            //    routeTemplate: "api/Operations/Programs/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "OriginsApi",
            //    routeTemplate: "api/Operations/Origins/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "ChannelTelemetryApi",
            //    routeTemplate: "api/Telemetry/ChannelTelemetry/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "ProgramTelemetryApi",
            //    routeTemplate: "api/Telemetry/ProgramTelemetry/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "EgressTelemetryApi",
            //    routeTemplate: "api/Telemetry/ProgramTelemetry/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            
            // Default catch-all
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ChildApi",
                routeTemplate: "api/accounts/{account}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "Action",
                routeTemplate: "api/accounts/{account}/{controller}/{id}/{action}");

            //setting up .json as the default response
            /* found this answer here: http://stackoverflow.com/questions/9847564/how-do-i-get-asp-net-web-api-to-return-json-instead-of-xml-using-chrome/20556625#20556625
             */
            var xmlHdrFormatter = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(xmlHdrFormatter);
        }
    }
}
