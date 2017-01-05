namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Reflection;
    using Microsoft.WindowsAzure.MediaServices.Client.RequestAdapters;

    public static class MediaServicesClassFactoryExtensions
    {
        public static readonly Version CurrentVersion = new Version(major: 2, minor: 12);

        public static IMediaDataServiceContext CreateCustomDataServiceContext(this MediaServicesClassFactory factory)
        {
            var azureFactory = (AzureMediaServicesClassFactory)factory;
            var serviceVersionAdapter = new ServiceVersionAdapter(CurrentVersion);

            typeof(AzureMediaServicesClassFactory)
               .GetField("_serviceVersionAdapter", BindingFlags.Instance | BindingFlags.NonPublic)
               .SetValue(azureFactory, serviceVersionAdapter);

            return azureFactory.CreateDataServiceContext();
        }
    }
}
