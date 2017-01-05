namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public static class MediaContextBaseExtensions
    {
        private const string AzureTableBaseEndPointAddress = "https://{0}.table.core.windows.net/";

        public static NotificationEndPointCollectionV212 GetNotificationEndPoints(this MediaContextBase context)
        {
            return new NotificationEndPointCollectionV212(context);
        }

        public static MonitoringSettingsCollection GetMonitoringSettings(this MediaContextBase context)
        {
            return new MonitoringSettingsCollection(context);
        }

        public static async Task<ServiceMetadata> GetServiceMetadataAsync(this MediaContextBase context)
        {
            var dataContext = context.MediaServicesClassFactory.CreateCustomDataServiceContext();
            var baseUri = dataContext.GetBaseUri();
            var uri = new Uri(string.Concat(baseUri.AbsoluteUri.TrimEnd('/'), "/$metadata"));

            if (string.IsNullOrWhiteSpace(context.Credentials.AccessToken))
            {
                context.Credentials.RefreshToken();
            }

            using (var client = new HttpClient())
            {
                var request = CreateRequest(HttpMethod.Get, uri, context.Credentials.AccessToken);
                var response = await client.SendAsync(request).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var metadata = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new ServiceMetadata(metadata);
            }
        }

        public static ServiceMetadata GetServiceMetadata(this MediaContextBase context)
        {
            try
            {
                return context.GetServiceMetadataAsync().Result;
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }

        public static string GetAzureTableEndPointAddress(this MediaContextBase context)
        {
            var azureTableEndPointAddress = string.Empty;
            var storageAccount = context.StorageAccounts.Where(s => s.IsDefault).FirstOrDefault();
            if (storageAccount != null)
            {
                azureTableEndPointAddress = string.Format(CultureInfo.InvariantCulture, AzureTableBaseEndPointAddress, storageAccount.Name);
            }

            return azureTableEndPointAddress;
        }

        public static async Task<IMonitoringSettings> CreateOrUpdateMonitoringSettingsAsync(this MediaContextBase context, string level)
        {
            var monitoringSettingsCollection = context.GetMonitoringSettings();

            // At most, one Monitoring Settings entity is allowed
            var monitoringSettings = monitoringSettingsCollection.SingleOrDefault();
            if (monitoringSettings != null)
            {
                foreach (var setting in monitoringSettings.Settings)
                {
                    setting.Level = level;
                }

                await monitoringSettings.UpdateAsync().ConfigureAwait(false);
            }
            else
            {
                var notificationEndPoint = await context.GetOrCreateDefaultAzureTableNotificationEndPointAsync().ConfigureAwait(false);
                var settings = new List<ComponentMonitoringSettings>
                {
                    new ComponentMonitoringSettings
                    {
                        Component = "Channel",
                        Level = level
                    },
                    new ComponentMonitoringSettings
                    {
                        Component = "StreamingEndpoint",
                        Level = level
                    }
                };

                monitoringSettings = await monitoringSettingsCollection.CreateAsync(notificationEndPoint.Id, settings).ConfigureAwait(false);
            }

            return monitoringSettings;
        }

        public static IMonitoringSettings CreateOrUpdateMonitoringSettings(this MediaContextBase context, string level)
        {
            try
            {
                return context.CreateOrUpdateMonitoringSettingsAsync(level).Result;
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }

        private static async Task<INotificationEndPointV212> GetOrCreateDefaultAzureTableNotificationEndPointAsync(this MediaContextBase context)
        {
            var notificationEndPointsCollection = context.GetNotificationEndPoints();
            var notificationEndPoint = notificationEndPointsCollection.Where(ne => ne.EndPointType == NotificationEndPointTypeV212.AzureTable).FirstOrDefault();

            if (notificationEndPoint == null)
            {
                notificationEndPoint = await notificationEndPointsCollection
                    .CreateAsync("monitoring", NotificationEndPointTypeV212.AzureTable, context.GetAzureTableEndPointAddress())
                    .ConfigureAwait(false);
            }

            return notificationEndPoint;
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, Uri uri, string accessToken)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Add("x-ms-version", MediaServicesClassFactoryExtensions.CurrentVersion.ToString());
            request.Headers.Add("DataServiceVersion", "3.0");
            request.Headers.Add("MaxDataServiceVersion", "3.0");
            request.Headers.ExpectContinue = false;

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return request;
        }
    }
}
