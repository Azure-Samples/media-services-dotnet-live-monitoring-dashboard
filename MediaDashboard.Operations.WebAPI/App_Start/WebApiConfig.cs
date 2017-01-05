using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MediaDashboard.Operations.WebAPI
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
                routeTemplate: "api/MediaServicesAccount/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //the accountName is used here as a visual cue to ensure the proper
            //WAMS deployment name is being used
            config.Routes.MapHttpRoute(
                name: "OperationsApi",
                routeTemplate: "api/Operations/MediaServicesAccount/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //setting up .json as the default response
            /* found this answer here: http://stackoverflow.com/questions/9847564/how-do-i-get-asp-net-web-api-to-return-json-instead-of-xml-using-chrome/20556625#20556625
             */
            var xmlHdrFormatter = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(xmlHdrFormatter);
        }
    }
}
