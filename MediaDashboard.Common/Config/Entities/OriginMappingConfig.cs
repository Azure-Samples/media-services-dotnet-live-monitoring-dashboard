using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class OriginMappingConfig
    {
        [JsonProperty("channelId")]
        public string ChannelId;

        [JsonProperty("originId")]
        public string OriginId;
    }
}