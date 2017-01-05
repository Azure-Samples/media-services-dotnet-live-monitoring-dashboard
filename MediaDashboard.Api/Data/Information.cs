using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Api.Data
{
    public class Information
    {
        [JsonProperty("dashboardTitle")]
        public string DashboardTitle {get;set;}

        [JsonProperty("contentProviders")]
        public List<AContentProvider> ContentProviders { get; set; }
    }
}