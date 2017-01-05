using Newtonsoft.Json;

namespace MediaDashboard.Api.Data
{
    public class PipelineStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }
}