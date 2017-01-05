using Newtonsoft.Json;

namespace MediaDashboard.Common.Data
{
    public class Metric
    {
        public Metric()
        {

        }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("Unit")]
        public string Unit { get; set; }

         [JsonProperty("DisplayUnit")]
        public string DisplayUnit { get; set; }

        [JsonProperty("AggregationType")]
        public string AggregationType { get; set; }

        [JsonProperty("GreenBoundaryValue")]
        public decimal? GreenBoundaryValue { get; set; }

        [JsonProperty("YellowBoundaryValue")]
        public decimal? YellowBoundaryValue { get; set; }

        [JsonProperty("RedBoundaryValue")]
        public decimal? RedBoundaryValue { get; set; }
    }
}
