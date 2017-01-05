using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class StorageAccountConfig
    {
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("accountKey")]
        public string AccountKey { get; set; }
    }
}