namespace Microsoft.WindowsAzure.MediaServices.Client
{
    /// <summary>
    /// The notification endpoint type.
    /// </summary>
    public enum NotificationEndPointTypeV212
    {
        /// <summary>
        /// No type specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// An Azure Queue.
        /// </summary>
        AzureQueue = 1,

        /// <summary>
        /// An Azure Table.
        /// </summary>
        AzureTable = 2
    }
}
