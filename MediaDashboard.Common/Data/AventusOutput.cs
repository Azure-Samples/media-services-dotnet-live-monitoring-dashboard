using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class AventusOutput
    {
        List<AventusPackager> _packagers;

        [JsonProperty("HealthLevel")]
        public string HealthLevel { get; set; }

        [JsonProperty("Packagers")]
        public List<AventusPackager> Packagers {
            get { return _packagers; }
            set { _packagers = value; }
        }


        public AventusOutput()
        {
            Packagers = new List<AventusPackager>();
        }
    }
}
