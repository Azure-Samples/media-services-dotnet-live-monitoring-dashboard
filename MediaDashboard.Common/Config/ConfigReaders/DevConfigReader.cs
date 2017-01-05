using MediaDashboard.Common.Config.Entities;
using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.ConfigReaders
{
    public class DevConfigReader : IConfigReader
    {
        // default custom config goes here.
        public const string DefaultDevConfig = @"{
            }";

        private string _devConfig;

        public DevConfigReader(string config = DefaultDevConfig)
        {
            _devConfig = config;
        }
           
        public MediaDashboardConfig ReadConfig()
        {
            return JsonConvert.DeserializeObject<MediaDashboardConfig>(_devConfig);
        }
    }
}