namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MediaServices.Client.TransientFaultHandling;

    public sealed class NotificationEndPointCollectionV212 : CloudBaseCollection<INotificationEndPointV212>
    {
        /// <summary>
        /// The entity set name for NotificationEndPoints.
        /// </summary>
        internal const string NotificationEndPoints = "NotificationEndPoints";

        internal NotificationEndPointCollectionV212(MediaContextBase cloudMediaContext)
            : base(cloudMediaContext)
        {
            Queryable = MediaContext.MediaServicesClassFactory.CreateCustomDataServiceContext().CreateQuery<INotificationEndPointV212, NotificationEndPointDataV212>(NotificationEndPoints);
        }

        /// <summary>
        /// Create a notification endpoint object in asynchronous mode.
        /// </summary>
        /// <param name="name">Name of notification endpoint</param>
        /// <param name="endPointType">Notification endpoint type</param>
        /// <param name="endPointAddress">Notification endpoint address</param>
        /// <returns>Task of creating notification endpoint.</returns>
        public Task<INotificationEndPointV212> CreateAsync(string name, NotificationEndPointTypeV212 endPointType, string endPointAddress)
        {
            NotificationEndPointDataV212 notificationEndPoint = new NotificationEndPointDataV212
            {
                Name = name,
                EndPointType = (int)endPointType,
                EndPointAddress = endPointAddress
            };

            notificationEndPoint.SetMediaContext(MediaContext);
            IMediaDataServiceContext dataContext = MediaContext.MediaServicesClassFactory.CreateCustomDataServiceContext();
            dataContext.AddObject(NotificationEndPoints, notificationEndPoint);

            MediaRetryPolicy retryPolicy = this.MediaContext.MediaServicesClassFactory.GetSaveChangesRetryPolicy(dataContext as IRetryPolicyAdapter);

            return retryPolicy.ExecuteAsync<IMediaDataServiceResponse>(() => dataContext.SaveChangesAsync(notificationEndPoint))
                .ContinueWith<INotificationEndPointV212>(
                    t =>
                    {
                        t.ThrowIfFaulted();

                        return (NotificationEndPointDataV212)t.Result.AsyncState;
                    },
                    TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Create a notification endpoint object.
        /// </summary>
        /// <param name="name">Name of notification endpoint</param>
        /// <param name="endPointType">Notification endpoint type</param>
        /// <param name="endPointAddress">Notification endpoint address</param>
        /// <returns>Notification endpoint object</returns>
        public INotificationEndPointV212 Create(string name, NotificationEndPointTypeV212 endPointType, string endPointAddress)
        {
            try
            {
                Task<INotificationEndPointV212> task = CreateAsync(name, endPointType, endPointAddress);
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