using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{

    public class AventusHealth
    {
        [JsonProperty("HealthLevel")]
        public string HealthLevel { get; set; }

        [JsonProperty("RuleInfo")]
        public List<AventusHealthRule> RuleInfo { get; set; }

        public AventusHealth()
        {
            RuleInfo = new List<AventusHealthRule>();
        }
    }
}
