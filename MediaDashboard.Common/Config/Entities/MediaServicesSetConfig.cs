using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class MediaServicesSetConfig
    {
        [JsonProperty("descriptiveName")]
        public string Name { get; set; }

        [JsonProperty("loggingAccounts")]
        public List<StorageAccountConfig> LoggingAccounts { get; set; }

        [JsonProperty("mediaServicesAccounts")]
        public List<MediaServicesAccountConfig> MediaServicesAccounts { get; set; }

        [JsonProperty("dataStorageConnections")]
        public List<AzureDataConfig> DataStorageConnections { get; set; }
    }
}