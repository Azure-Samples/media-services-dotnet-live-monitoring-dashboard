using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaDashboard.Api.Data
{
    public class ADeployment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("accountname")]
        public string AccountName { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("channels")]
        public List<AChannel> Channels { get; set; }
    }
}