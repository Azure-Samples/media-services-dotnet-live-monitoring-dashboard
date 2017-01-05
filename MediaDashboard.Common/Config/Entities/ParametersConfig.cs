using MediaDashboard.Common.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Config.Entities
{
    /// <summary>
    /// Parameter configuration that is not really related to content, but more to the behavior of the services we are using or monitoring
    /// i.e. threasholds, capacity, etc.
    /// </summary>
    public class ParametersConfig
    {

        public ParametersConfig()
        {
            ReservedUnitConfig = new ReservedUnitConfig();
            MonitoringIntervalConfig = new MonitoringIntervalConfig();
            FragmentConfig = new FragmentConfig();
            Origin = new OriginParameterConfig();
            HLSConfig = new HLSRatioConfig();
            EncodingConfig = new EncodingPresetConfig();
        }

        [JsonProperty("defaultMetricThresholds")]
        public Metric[] MetricThresholds
        {
            get
            {
                return Metrics?.Values.ToArray();
            }
            set
            {
                if (value != null)
                {
                    Metrics = value.ToDictionary(m => m.Name, m => m);
                }
            }
        }

        [JsonProperty("reservedUnit")]
        public ReservedUnitConfig ReservedUnitConfig { get; set; }

        [JsonProperty("defaultFragmentDuration")]
        public FragmentConfig FragmentConfig { get; set; }

        [JsonProperty("encodingPreset")]
        public EncodingPresetConfig EncodingConfig { get; set; }

        [JsonProperty("hlsRatio")]
        public HLSRatioConfig HLSConfig { get; set; }

        [JsonProperty("defaultMonitoringIntervalSeconds")]
        public MonitoringIntervalConfig MonitoringIntervalConfig { get; set; }

        [JsonProperty("origin")]
        public OriginParameterConfig Origin { get; set; }

        [JsonIgnore]
        public Dictionary<string, Metric> Metrics { get; private set; }
    }
}