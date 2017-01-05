namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The monitoring settings for serialization.
    /// </summary>
    public interface IMonitoringSettings
    {
        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets or sets the notification endpoint ID.
        /// </summary>
        string NotificationEndPointId { get; set; }

        DateTime Created { get; }

        DateTime LastModified { get; }

        /// <summary>
        /// Gets or sets the component monitoring settings.
        /// </summary>
        IList<ComponentMonitoringSettings> Settings { get; set; }

        Task UpdateAsync();

        void Update();

        Task DeleteAsync();

        void Delete();
    }
}