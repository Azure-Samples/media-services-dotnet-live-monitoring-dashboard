using Newtonsoft.Json;
using System.ComponentModel;

namespace MediaDashboard.Common.Config.Entities
{
    public class ReservedUnitConfig
    {
        const int DefaultCapacity = 200;

        public ReservedUnitConfig()
        {
            Capacity = DefaultCapacity;
        }

        [JsonProperty(propertyName:"capacity", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultCapacity)]
        public int Capacity { get; set; }
    }
}