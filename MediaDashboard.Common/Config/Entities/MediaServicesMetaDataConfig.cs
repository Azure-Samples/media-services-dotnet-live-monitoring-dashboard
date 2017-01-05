using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class MediaServicesMetaDataConfig
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("resourcegroup")]
        public string ResourceGroup { get; set; }

        [JsonProperty("azureSubscriptionId")]
        public string AzureSubscriptionId { get; set; }

        [JsonProperty("acsScope")]
        public string AcsScope { get; set; }

        [JsonProperty("acsBaseAddress")]
        public string AcsBaseAddress { get; set; }

        [JsonProperty("certThumbprint")]
        public string Thumbprint { get; set; }

        [JsonProperty("aventusDNSBase")]
        public string aventusDNSBase { get; set; }

        [JsonProperty("aoaApiServer")]
        public string AoAApiServer { get; set; }
    }
}