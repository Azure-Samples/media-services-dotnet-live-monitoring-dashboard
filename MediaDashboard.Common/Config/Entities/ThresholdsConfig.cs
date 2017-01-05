using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class ThresholdsConfig
    {
        [JsonProperty("warning")]
        public int Warning { get; set; }

        [JsonProperty("error")]
        public int Error { get; set; }
    }
}