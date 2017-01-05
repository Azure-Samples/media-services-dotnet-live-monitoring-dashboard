using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{

    public class AventusTelemetry
    {

        [JsonProperty("Metrics")]
        public List<IMetricBase> Metrics { get; set; }

        [JsonProperty("ChannelId")]
        public string ChannelId { get; set; }

        [JsonProperty("ChannelName")]
        public string ChannelName { get; set; }

        [JsonProperty("ChannelClockTime")]
        public DateTime LastRunTime { get; set; }

        [JsonProperty("FirstPublishTime")]
        public DateTime FirstPublishTime { get; set; }

        [JsonProperty("RunId")]
        public string RunId { get; set; }

        [JsonProperty("Health")]
        public AventusHealth ChannelHealth { get; set; }

        [JsonProperty("AdMarkers")]
        public AventusAdMarker LastInsertedAdvertisement { get; set; }

        [JsonProperty("Slates")]
        public AventusSlate LastInsertedSlate { get; set; }

        [JsonProperty("Transcoders")]
        public AventusTranscoder Transcoders { get; set; }

        [JsonProperty("Outputs")]
        public AventusOutput Outputs { get; set; }

        [JsonProperty("Inputs")]
        public AventusInput Inputs { get; set; }

        public AventusTelemetry()
        {
            Metrics = new List<IMetricBase>();

        }
    }
}
