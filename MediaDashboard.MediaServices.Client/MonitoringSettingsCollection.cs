namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MediaServices.Client.TransientFaultHandling;

    public class MonitoringSettingsCollection : BaseCollection<IMonitoringSettings>
    {
        public const string MonitoringConfigurationsSet = "MonitoringConfigurations";

        private readonly Lazy<IQueryable<IMonitoringSettings>> filterQuery;

        internal MonitoringSettingsCollection(MediaContextBase context)
            : base(context)
        {
            this.filterQuery = new Lazy<IQueryable<IMonitoringSettings>>(
                () => 
                {
                    var dataContext = this.MediaContext
                        .MediaServicesClassFactory
                        .CreateCustomDataServiceContext();

                    return dataContext.CreateQuery<IMonitoringSettings, MonitoringSettingsData>(MonitoringConfigurationsSet);
                });
        }

        protected override IQueryable<IMonitoringSettings> Queryable
        {
            get { return this.filterQuery.Value; }
            set { throw new NotSupportedException(); }
        }

        public Task<IMonitoringSettings> CreateAsync(string notificationEndPointId, IList<ComponentMonitoringSettings> settings)
        {
            var monitoringSettingsData = new MonitoringSettingsData
            {
                NotificationEndPointId = notificationEndPointId,
                Settings = settings
            };

            monitoringSettingsData.SetMediaContext(this.MediaContext);

            var dataContext = this.MediaContext.MediaServicesClassFactory.CreateCustomDataServiceContext();
            dataContext.AddObject(MonitoringConfigurationsSet, (IMonitoringSettings)monitoringSettingsData);

            var retryPolicy = this.MediaContext.MediaServicesClassFactory.GetSaveChangesRetryPolicy(dataContext as IRetryPolicyAdapter);

            return retryPolicy.ExecuteAsync<IMediaDataServiceResponse>(() => dataContext.SaveChangesAsync(monitoringSettingsData))
                .ContinueWith<IMonitoringSettings>(
                    t =>
                    {
                        t.ThrowIfFaulted();

                        var data = (IMonitoringSettings)t.Result.AsyncState;
                        
                        return data;
                    });
        }

        public IMonitoringSettings Create(string notificationEndPointId, IList<ComponentMonitoringSettings> settings)
        {
            try
            {
                Task<IMonitoringSettings> task = CreateAsync(notificationEndPointId, settings);
                task.Wait();

                return task.Result;
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }
    }
}
