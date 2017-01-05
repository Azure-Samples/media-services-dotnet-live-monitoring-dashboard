using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class ContentProviderConfig
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mediaServicesSets")]
        public List<MediaServicesSetConfig> MediaServicesSets { get; set; }


    }
}