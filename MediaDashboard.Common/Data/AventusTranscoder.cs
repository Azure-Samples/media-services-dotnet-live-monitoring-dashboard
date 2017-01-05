using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class AventusTranscoder
    {
        List<AventusVideoDecoder> _videoDecoders;
        List<AventusAudioDecoder> _audioDecoders;

        [JsonProperty("AudioDecoders")]
        public  List<AventusAudioDecoder> AudioDecoders
        {
            get { return _audioDecoders; }
            set { _audioDecoders = value; }
        }

         [JsonProperty("VideoDecoders")]
        public List<AventusVideoDecoder> VideoDecoders
        {
            get { return _videoDecoders; }
            set { _videoDecoders = value; }
        }

        [JsonProperty("HealthLevel")]
        public string HealthLevel { get; set; }

        public AventusTranscoder()
        {
            VideoDecoders = new List<AventusVideoDecoder>();
            AudioDecoders = new List<AventusAudioDecoder>();
        }
    }
}
