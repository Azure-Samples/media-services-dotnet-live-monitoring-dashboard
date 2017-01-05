using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class MediaProgram
    {
        [JsonProperty("Health")]
        public HealthStatus Health { get; set; }

        [JsonProperty("AssetId")]
        public string AssetId { get; set; }

        [JsonProperty("ChannelId")]
        public string ChannelId { get; set; }

        [JsonProperty("Created")]
        public DateTime Created { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("LastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("ManifestName")]
        public string ManifestName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        public TimeSpan ArchiveWindowLength { get; set; }

        [JsonProperty("LastMetricUpdate")]
        public DateTime LastMetricUpdate;

        public List<IMetricBase> Metrics;

        public MediaProgram()
        {
            this.Metrics = new List<IMetricBase>();
        }

        public override string ToString()
        {
            return GetJsonString();
        }

        private string GetJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public HealthStatus OriginHealth { get; set; }

        public HealthStatus ArchiveHealth { get; set; }
    }
}
