namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// Media Services service metadata.
    /// </summary>
    public class ServiceMetadata
    {
        private readonly XDocument _metadata;
        private readonly XmlNamespaceManager _namespaceManager;

        /// <summary>
        /// Initializes a new instance of the ServiceMetadata class.
        /// </summary>
        /// <param name="metadata">The service metadata.</param>
        internal ServiceMetadata(string metadata)
        {
            _metadata = XDocument.Parse(metadata);

            _namespaceManager = new XmlNamespaceManager(new NameTable());
            _namespaceManager.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2007/06/edmx");
            _namespaceManager.AddNamespace("schema", "http://schemas.microsoft.com/ado/2009/11/edm");
        }

        /// <summary>
        /// Gets a value indicating whether Monitoring Configuration is enabled in the service metadata.
        /// </summary>
        public bool IsMonitoringEnabled
        {
            get { return _metadata.XPathSelectElement("/edmx:Edmx/edmx:DataServices/schema:Schema/schema:EntityType[@Name='MonitoringConfiguration']", _namespaceManager) != null; }
        }
    }
}