using Newtonsoft.Json;

namespace MediaDashboard.Api.Data
{
    public class AProgram
    {
        [JsonProperty("asset")]
        public AAsset Asset { get; set; }
    }
}