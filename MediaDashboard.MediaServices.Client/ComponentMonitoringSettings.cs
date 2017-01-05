namespace Microsoft.WindowsAzure.MediaServices.Client
{
    /// <summary>
    /// The monitoring settings for a component.
    /// </summary>
    public class ComponentMonitoringSettings
    {
        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets or sets the monitoring level.
        /// </summary>
        public string Level { get; set; }
    }
}