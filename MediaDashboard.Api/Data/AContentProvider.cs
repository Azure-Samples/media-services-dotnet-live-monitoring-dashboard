using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaDashboard.Api.Data
{
    public class AContentProvider
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mediaPipelines")]
        public List<MediaPipeline> MediaPipelines { get; set; }
    }
}