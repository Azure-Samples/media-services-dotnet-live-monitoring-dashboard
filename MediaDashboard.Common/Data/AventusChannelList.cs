using Newtonsoft.Json;

namespace MediaDashboard.Common.Data
{
    public class AventusChannelList
    {
        [JsonProperty("channelList")]
        public AventusChannel[] ChannelList { get; set; }
    }
}
