using Newtonsoft.Json;
using System.ComponentModel;

namespace MediaDashboard.Common.Config.Entities
{
    public class EncodingPresetConfig
    {
        private const string DefaultEncodingPreset = "Default720p";

        [JsonProperty(propertyName: "presetName", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultEncodingPreset)]
        public string EncodingPresetName { get; set; }
    }
}
