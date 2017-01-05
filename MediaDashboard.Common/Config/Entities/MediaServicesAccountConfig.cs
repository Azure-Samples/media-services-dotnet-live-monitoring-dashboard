using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class MediaServicesAccountConfig
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("accountKey")]
        public string AccountKey { get; set; }

        [JsonProperty("adminUrl")]
        public Uri AdminUri { get; set; }

        [JsonProperty("metaData")]
        public MediaServicesMetaDataConfig MetaData { get; set; }

        // Mapping of channels to origins
        [JsonProperty("originMappings")]
        public List<OriginMappingConfig> OriginMappings { get; set; }

        [JsonProperty("telemetryStorage")]
        public StorageAccountConfig TelemetryStorage { get; set; }
    }
}