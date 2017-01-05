using Newtonsoft.Json;
using System.ComponentModel;

namespace MediaDashboard.Common.Config.Entities
{
    public class MonitoringIntervalConfig
    {
        const int DefaultInterval = 30;

        public MonitoringIntervalConfig()
        {
            DefaultIntervalSeconds = DefaultInterval;
        }

        [JsonProperty(propertyName: "interval", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultInterval)]
        public int DefaultIntervalSeconds { get; set; }
    }
}