namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System.Threading.Tasks;

    /// <summary>
    /// Notification endpoint, to which the publisher pushes notification, from which
    /// the subscriber reads notification.
    /// 
    /// The endpoint is provided by the application.
    /// </summary>
    public interface INotificationEndPointV212
    {
        /// <summary>
        /// Unique identifier of notification endpoint
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Display name of the notification endpoint.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Type of notification endpoint.
        /// 
        /// Media service uses this type to determine how to write the notification to the endpoint. 
        /// </summary>
        NotificationEndPointTypeV212 EndPointType { get; }

        /// <summary>
        /// Address of endpoint. The constraints of this value is determined by the endpoint type.
        /// </summary>
        string EndPointAddress { get; }

        /// <summary>
        /// Update the notification endpoint object.
        /// </summary>
        void Update();

        /// <summary>
        /// Update the notification endpoint object in asynchronous mode.
        /// </summary>
        /// <returns>Task of updating the notification endpoint.</returns>
        Task UpdateAsync();

        /// <summary>
        /// Delete this instance of notification endpoint object.
        /// </summary>
        void Delete();

        /// <summary>
        /// Delete this instance of notification endpoint object in asynchronous mode.
        /// </summary>
        /// <returns>Task of deleting the notification endpoint.</returns>
        Task DeleteAsync();
    }
}