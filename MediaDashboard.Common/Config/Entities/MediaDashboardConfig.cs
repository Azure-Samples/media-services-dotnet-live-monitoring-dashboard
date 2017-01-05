using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class MediaDashboardConfig
    {
        [JsonProperty("sys")]
        public SysConfig Sys { get; set; }

        [JsonProperty("content")]
        public ContentConfig Content { get; set; }

        [JsonProperty("parameters")]
        public ParametersConfig Parameters { get; set; }
    }
}