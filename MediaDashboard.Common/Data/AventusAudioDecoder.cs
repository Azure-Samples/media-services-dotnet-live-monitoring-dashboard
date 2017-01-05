using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class AventusAudioDecoder : IAventusDecoder
    {
        List<AventusAudioEncoder> _audioEncoders;

        

        [JsonProperty("Codec")]
        public string Codec { get; set; }

        [JsonProperty("Pid")]
        public int Pid { get; set; }

         [JsonProperty("TotalAudioLostDuration")]
        public long TotalLostDuration { get; set; }

        [JsonProperty("BitRate")]
        public long BitRate { get; set; }

        [JsonProperty("ExpectedBitRate")]
        public long ExpectedBitRate { get; set; }

         [JsonProperty("TaskIndex")]
        public int TaskIndex { get; set; }

        [JsonProperty("TaskGroupId")]
        public string TaskGroupId { get; set; }

        [JsonProperty("Health")]
        public AventusHealth Health { get; set; }

        [JsonProperty("SampleRate")]
        public int SampleRate { get; set; }

        [JsonProperty("TotalAccessUnitsDecoded")]
        public int TotalAccessUnitsDecoded { get; set; }

        [JsonProperty("AudioEncoders")]
        public List<AventusAudioEncoder> AudioEncoders
        {
            get { return _audioEncoders; }
            set { _audioEncoders = value; }
        }

        public AventusAudioDecoder()
        {
            AudioEncoders = new List<AventusAudioEncoder>();
        }

    }
}
