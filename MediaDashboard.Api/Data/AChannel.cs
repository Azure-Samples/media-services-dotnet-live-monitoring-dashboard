using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Api.Data
{
    public class AChannel
    {
        [JsonProperty("programs")]
        public List<AProgram> Programs { get; set; }

        [JsonProperty("origin")]
        public AOrigin Origin { get; set; }
    }
}