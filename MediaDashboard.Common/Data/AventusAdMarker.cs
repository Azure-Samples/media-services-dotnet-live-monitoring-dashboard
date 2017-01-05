using Newtonsoft.Json;
using System;

namespace MediaDashboard.Common.Data
{
    public class AventusAdMarker
    {
        [JsonProperty("LastReceivedTime")]
        public DateTime LastReceivedTime { get; set; }

        [JsonProperty("LastAdMarkerID")]
        public long LastAdMarkerID { get; set; }

        [JsonProperty("LastAdMarkerDuration")]
        public int LastAdMarkerDuration { get; set; }
    }
}
