using MediaDashboard.Common.Config.Entities;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MediaDashboard.Common.Config.ConfigReaders
{
    public class BlobConfigReader : IConfigReader
    {
        private readonly string _configStorageConnectionString;
        private readonly string _configContainerName;
        private readonly string _configBlobName;

        public BlobConfigReader(string configStorageConnectionString, string configContainerName, string configBlobName)
        {
            _configStorageConnectionString = configStorageConnectionString;
            _configContainerName = configContainerName;
            _configBlobName = configBlobName;
        }

        public MediaDashboardConfig ReadConfig()
        {
            var account = CloudStorageAccount.Parse(_configStorageConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(_configContainerName);
            var blob = container.GetBlockBlobReference(_configBlobName);

            Trace.TraceInformation("[BlobConfigReader] Reading config {0} in {1}", blob.Name, container.Name);

            var config = blob.DownloadText();

            return JsonConvert.DeserializeObject<MediaDashboardConfig>(config);
        }
    }
}