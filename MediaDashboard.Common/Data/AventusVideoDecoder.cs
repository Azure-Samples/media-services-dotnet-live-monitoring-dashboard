using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class AventusVideoDecoder : IAventusDecoder
    {
        List<AventusVideoEncoder> _videoEncoders;
        #region from IAventusDecoder
        [JsonProperty("BitRate")]
        public long BitRate
        {
            get;  set;
        }

        [JsonProperty("ExpectedBitRate")]
        public long ExpectedBitRate
        {
            get;
            set;
        }

        [JsonProperty("Health")]
        public AventusHealth Health
        {
            get;
            set;
        }

        [JsonProperty("TaskIndex")]
        public int TaskIndex
        {
            get;
            set;
        }

        [JsonProperty("TaskGroupId")]
        public string TaskGroupId
        {
            get;
            set;
        }

        [JsonProperty("Codec")]
        public string Codec { get; set; }

        [JsonProperty("Pid")]
        public int Pid { get; set; }

        [JsonProperty("TotalVideoLostDuration")]
        public long TotalLostDuration { get; set; }
        #endregion

        [JsonProperty("FrameRate")]
        public long FrameRate{ get; set; }

        [JsonProperty("Width")]
        public int Width { get; set; }

        [JsonProperty("Height")]
        public int Height { get; set; }

        [JsonProperty("EncoderName")]
        public string EncoderName { get; set; }

        [JsonProperty("TotalFramesEncoded")]
        public int TotalFramesEncoded { get; set; }

        [JsonProperty("ExpectedFrameRate")]
        public long ExpectedFrameRate { get; set; }

        [JsonProperty("VideoEncoders")]
        public List<AventusVideoEncoder> VideoEncoders { get { return _videoEncoders; } set { _videoEncoders = value; } }

        public AventusVideoDecoder()
        {
            VideoEncoders = new List<AventusVideoEncoder>();
        }
    }
}
