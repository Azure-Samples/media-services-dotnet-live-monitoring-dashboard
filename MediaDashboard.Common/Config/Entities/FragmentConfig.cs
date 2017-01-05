using Newtonsoft.Json;
using System.ComponentModel;

namespace MediaDashboard.Common.Config.Entities
{
    public class FragmentConfig
    {
        private const int DefaultFragmentDuration = 2;

        public FragmentConfig()
        {
            Duration = DefaultFragmentDuration;
        }

        [JsonProperty(propertyName:"duration", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultFragmentDuration)]
        public int Duration { get; set; }
    }
}