using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class AventusInput
    {
        [JsonProperty("HealthLevel")]
        public string HealthLevel { get; set; }

        [JsonProperty("Receivers")]
        public List<AventusReceiver> Receivers { get; set; }

        public AventusInput()
        {
            Receivers = new List<AventusReceiver>();
        }
    }
}
