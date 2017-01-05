using Newtonsoft.Json;

namespace MediaDashboard.Api.Data
{
    public class Status
    {
        [JsonProperty("channelStatus")]
        public int ChannelStatus { get; set; }

        [JsonProperty("programStatus")]
        public int ProgramStatus { get; set; }

        [JsonProperty("originStatus")]
        public int OriginStatus { get; set; }

    }
}