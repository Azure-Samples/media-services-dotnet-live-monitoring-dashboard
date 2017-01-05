using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    /// <summary>
    /// Content related configuration (except static parameters)
    /// i.e. things that will be "visible" in the dashboard at runtime in some way or another
    /// </summary>
    public class ContentConfig
    {
        [JsonProperty("dashboardTitle")]
        public string DashboardTitle { get; set; }

        [JsonProperty("contentProviders")]
        public List<ContentProviderConfig> ContentProviders { get; set; }

        [JsonProperty("parameters")]
        public ParametersConfig Parameters { get; set; }
    }
}