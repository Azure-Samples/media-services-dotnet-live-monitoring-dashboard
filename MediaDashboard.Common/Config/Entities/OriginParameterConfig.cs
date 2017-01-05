using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class OriginParameterConfig
    {
        [JsonProperty("thresholds")]
        public ThresholdsConfig Thresholds { get; set; }
    }
}