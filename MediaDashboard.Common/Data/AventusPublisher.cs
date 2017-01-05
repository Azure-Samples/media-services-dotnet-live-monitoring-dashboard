using Newtonsoft.Json;
using System;

namespace MediaDashboard.Common.Data
{
    public class AventusPublisher: IAventusObject
    {
        [JsonProperty("PublishingUrl")]
        public string PublishingUrl { get; set; }

        [JsonProperty("Protocol")]
        public string Protocol { get; set; }

        [JsonProperty("PublishingQueueCapacity")]
        public long PublishingQueueCapacity { get; set; }

        [JsonProperty("PublishingQueueDuration")]
        public long PublishingQueueDuration { get; set; }

        [JsonProperty("EgressBitRate")]
        public long EgressBitRate { get; set; }

        [JsonProperty("TotalDisconnectCount")]
        public int TotalDisconnectCount { get; set; }

        [JsonProperty("TotalConnectFailCount")]
        public int TotalConnectFailCount { get; set; }

        [JsonProperty("LastDisconnectTime")]
        public DateTime LastDisconnectTime { get; set; }

        [JsonProperty("Health")]
        public AventusHealth Health { get; set; }
    }
}
