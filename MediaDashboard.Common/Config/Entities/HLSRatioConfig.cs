using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MediaDashboard.Common.Config.Entities
{
    public class HLSRatioConfig
    {
        private const int DefaultRatioConfig = 1;

        public HLSRatioConfig()
        {
            HLSRatio = DefaultRatioConfig;
        }

        [JsonProperty(propertyName: "defaultRatio", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultRatioConfig)]
        public int HLSRatio { get; set; }
    }
}
