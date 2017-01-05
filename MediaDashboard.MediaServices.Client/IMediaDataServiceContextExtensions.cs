namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Data.Services.Client;
    using System.Reflection;

    public static class IMediaDataServiceContextExtensions
    {
        public static Uri GetBaseUri(this IMediaDataServiceContext mediaDataServiceContext)
        {
            var dataContext = mediaDataServiceContext.GetType()
               .GetField("_dataContext", BindingFlags.Instance | BindingFlags.NonPublic)
               .GetValue(mediaDataServiceContext) as DataServiceContext;

            return dataContext.BaseUri;
        }
    }
}
