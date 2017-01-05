using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Api.Models
{
    public class MediaServiceSetMetric
    {
        [JsonProperty("descriptiveName")]
        public string Name { get; set; }


        [JsonProperty("mediaServicesAccounts")]
        public List<AmsAccountMetricRepo> AmsAccounts { get; set; }
    }
}