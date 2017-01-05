using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class MediaChannel
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Health")]
        public HealthStatus Health { get; set; }

        [JsonProperty("Created")]
        public DateTime Created { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }    
      
        [JsonProperty("LastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("EncodingType")]
        public string EncodingType { get; set; }

        [JsonProperty("EncodingPreset")]
        public string EncodingPreset { get; set; }

        [JsonProperty("IngestUrls")]
        public string[] IngestUrls { get; set; }

        [JsonProperty("PreviewUrl")]
        public string PreviewUrl { get; set; }

        public string IngestAllowList { get; set; }

        public string PreviewAllowList { get; set; }

        public string PlaybackAllowList { get; set; }

        public List<MediaProgram> Programs { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("DefaultSlate")]
        public string DefaultSlate { get; set; }

        [JsonProperty("LastUpdate")]
        public DateTime LastUpdate;

        public string NameShort { get; set; }

        public string RunningTime { get; set; }

        public HealthStatus IngestHealth { get; set; }

        public HealthStatus EncodingHealth { get; set; }

        public HealthStatus ArchiveHealth { get; set; }

        public HealthStatus OriginHealth { get; set; }

        public string ThumbnailUrl { get; set; }

        [JsonProperty("FragmentDuration")]
        public int? FragmentDuration { get; set; }

        [JsonProperty("HLSPackingRatio")]
        public short? HLSPackingRatio { get; set; }

        [JsonProperty("ClientFragDiff")]
        public bool ClientFragDiff { get; set; }

        [JsonProperty("ClientEncodingDiff")]
        public bool ClientEncodingDiff { get; set; }

        [JsonProperty("ClientPackingRatioDiff")]
        public bool ClientPackingRatioDiff { get; set; }

        public string OriginId { get; set; }  

        /// <summary>
        /// Hostname of the mapped origin for this channel
        /// </summary>
        public string OriginHostName { get; set; }
    }
}
