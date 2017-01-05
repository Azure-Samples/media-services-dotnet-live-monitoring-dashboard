using Newtonsoft.Json;

namespace MediaDashboard.Api.Data
{
    public class AmsAccountStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}