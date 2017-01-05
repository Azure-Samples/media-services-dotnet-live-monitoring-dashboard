using Newtonsoft.Json;
using System;

namespace MediaDashboard.Common.Data
{
    public class AventusReceiver
    {
        [JsonProperty("BitRate")]
        public long BitRate { get; set; }

        [JsonProperty("SourceUrl")]
        public string SourceUrl { get; set; }

        [JsonProperty("LastReceivedTime")]
        public DateTime LastReceivedTime { get; set; }

        [JsonProperty("TotalBytesReceived")]
        public long TotalBytesReceived { get; set; }

        [JsonProperty("TotalBytesLost")]
        public long TotalBytesLost { get; set; }

        [JsonProperty("RecentBytesLost")]
        public long RecentBytesLost { get; set; }

        [JsonProperty("LastReceivedPTS")]
        public long LastReceivedPTS { get; set; }

        [JsonProperty("IngestFormat")]
        public string IngestFormat { get; set; }

        [JsonProperty("IngestProtocol")]
        public string IngestProtocol { get; set; }

        [JsonProperty("Health")]
        public AventusHealth Health { get; set; }

        [JsonProperty("TaskIndex")]
        public int TaskIndex { get; set; }

        [JsonProperty("TaskGroupId")]
        public string TaskGroupId { get; set; }

    }
}
