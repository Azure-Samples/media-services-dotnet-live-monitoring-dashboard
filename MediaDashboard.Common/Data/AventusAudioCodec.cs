using Newtonsoft.Json;

namespace MediaDashboard.Common.Data
{
    public class AventusVideoCodecConfig : ICodecConfig
    {
        [JsonProperty("H264Profile")]
        public string H264Profile { get; set; }

        [JsonProperty("BufferDuration")]
        public int BufferDuration { get; set; }

        [JsonProperty("GopMaxDuration")]
        public int GopMaxDuration { get; set; }

        [JsonProperty("NumReferenceFrames")]
        public int NumReferenceFrames { get; set; }

        [JsonProperty("NumBFrames")]
        public int NumBFrames { get; set; }

         [JsonProperty("Type")]
        public string EncoderType { get; set; }
    }
    public class AventusAudioCodecConfig : ICodecConfig
    {
         [JsonProperty("Type")]
        public string EncoderType { get; set; }
    }
}
