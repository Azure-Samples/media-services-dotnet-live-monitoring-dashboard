using Newtonsoft.Json;
using System.ComponentModel;

namespace MediaDashboard.Common.Config.Entities
{
    /// <summary>
    /// System related configuration i.e. things that are needed to get the system up and running
    /// </summary>
    public class SysConfig
    {
        [JsonProperty("cache")]
        public CacheConfig Cache { get; set; }

        [JsonProperty("maintenance")]
        public MaintenanceConfig Maintenance { get; set; }
    }

    public class MaintenanceConfig
    {
        //by default 30 days retention period.
        public const int DefaultRentionPeriodInDays = 30;

        [DefaultValue(30)]
        [JsonProperty("sqlRetentionPeriod")]
        public int SqlRentionPeriod { get; set; }

        [DefaultValue(30)]
        [JsonProperty("tableStorageRetentionPeriod")]
        public int TableStorageRetentionPeriod { get; set; }
    }
}
