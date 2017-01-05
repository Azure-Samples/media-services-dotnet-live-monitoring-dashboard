using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Api.Data
{
    public class MediaPipeline
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("deployments")]
        public List<ADeployment> Deployments { get; set; }
    }
}