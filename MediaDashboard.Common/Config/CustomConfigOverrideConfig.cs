using Newtonsoft.Json;

namespace MediaDashboard.Common.Config
{
    public class CustomConfigOverrideConfig
    {
        [JsonProperty("configType")]
        public string ConfigType { get; set; }

        [JsonProperty("blobConnectionString")]
        public string BlobConnectionString { get; set; }

        [JsonProperty("blobContainer")]
        public string BlobContainer { get; set; }

        [JsonProperty("blobName")]
        public string BlobName { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }
    }
}