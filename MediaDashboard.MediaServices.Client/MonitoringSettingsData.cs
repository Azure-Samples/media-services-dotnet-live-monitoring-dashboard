namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MediaServices.Client.TransientFaultHandling;

    /// <summary>
    /// The monitoring settings for serialization.
    /// </summary>
    [DataServiceKey("Id")]
    public class MonitoringSettingsData : BaseEntity<IMonitoringSettings>, IMonitoringSettings
    {
        public MonitoringSettingsData()
        {
            this.Settings = new List<ComponentMonitoringSettings>();
        }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the notification endpoint ID.
        /// </summary>
        public string NotificationEndPointId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the component monitoring settings.
        /// </summary>
        public IList<ComponentMonitoringSettings> Settings { get; set; }

        public Task UpdateAsync()
        {
            var mediaContext = this.GetMediaContext();
            var dataContext = mediaContext.MediaServicesClassFactory.CreateCustomDataServiceContext();
            dataContext.AttachTo(this.GetEntitySetName(), this);
            dataContext.UpdateObject(this);

            var retryPolicy = mediaContext.MediaServicesClassFactory.GetSaveChangesRetryPolicy(dataContext as IRetryPolicyAdapter);

            return retryPolicy.ExecuteAsync<IMediaDataServiceResponse>(() => dataContext.SaveChangesAsync(this))
                .ContinueWith<MonitoringSettingsData>(
                    t =>
                    {
                        t.ThrowIfFaulted();
                        var data = (MonitoringSettingsData)t.Result.AsyncState;
                        return data;
                    });
        }

        public void Update()
        {
            try
            {
                UpdateAsync().Wait();
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }

        public Task DeleteAsync()
        {
            var mediaContext = this.GetMediaContext();
            var dataContext = mediaContext.MediaServicesClassFactory.CreateCustomDataServiceContext();
            dataContext.AttachTo(this.GetEntitySetName(), this);

            dataContext.DeleteObject(this);

            var retryPolicy = mediaContext.MediaServicesClassFactory.GetSaveChangesRetryPolicy(dataContext as IRetryPolicyAdapter);

            return retryPolicy.ExecuteAsync<IMediaDataServiceResponse>(() => dataContext.SaveChangesAsync(this));
        }

        public void Delete()
        {
            try
            {
                DeleteAsync().Wait();
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
        }

        protected virtual string GetEntitySetName()
        {
            return MonitoringSettingsCollection.MonitoringConfigurationsSet;
        }
    }
}