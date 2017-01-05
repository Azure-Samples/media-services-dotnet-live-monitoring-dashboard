using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class AventusChannel
    {
        [JsonProperty("RunId")]
        public string RunId { get; set; }

        [JsonProperty("ChannelId")]
        public string AventusChannelId { get; set; }
 
        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("ContentProtectionEnabled")]
        public bool ContentProtectionEnabled { get; set; }

        [JsonProperty("ClosedCaptionsPresent")]
        public bool ClosedCaptionsPresent { get; set; }

        [JsonProperty("StreamPackageFormats")]
        public string[] StreamPackageFormats { get; set; }

        [JsonProperty("MaxVideoResolution")]
        public Dictionary<string, int> AllowedResolutions { get; set; }

        [JsonProperty("Health")]
        public string OverallHealth { get; set; }

        [JsonProperty("href")]
        public string BaseUrl { get; set; }

        public AventusTelemetry TelemetryResult { get; set; }
       
        public override string ToString()
        {
            return GetJsonString(); 
        }

        private string GetJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
