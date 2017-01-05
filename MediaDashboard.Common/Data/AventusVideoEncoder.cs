using Newtonsoft.Json;

namespace MediaDashboard.Common.Data
{
    public class AventusVideoEncoder : IAventusEncoder
    {

        [JsonProperty("EncoderName")]
        public string EncoderName { get; set; }

        [JsonProperty("BitRate")]
        public long BitRate { get; set; }

        [JsonProperty("FrameRate")]
        public long FrameRate { get; set; }

        [JsonProperty("ExpectedBitRate")]
        public long ExpectedBitRate { get; set; }

        [JsonProperty("TaskIndex")]
        public int TaskIndex { get; set; }

        [JsonProperty("TaskGroupId")]
        public string TaskGroupId { get; set; }

        [JsonProperty("Health")]
        public AventusHealth Health { get; set; }
    }
}
