using Newtonsoft.Json;
using System;

namespace MediaDashboard.Common.Data
{
    public class AventusSlate
    {
        [JsonProperty("LastInsertedTime")]
        public DateTime LastInsertedTime { get; set; }

        [JsonProperty("LastSlateDuration")]
        public int LastSlateDuration { get; set; }
    }
}
