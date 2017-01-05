using Newtonsoft.Json;

namespace MediaDashboard.Api.Data
{
    public class AAsset
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
