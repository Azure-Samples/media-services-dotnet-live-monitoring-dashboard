using System.IO;
using MediaDashboard.Common.Config.Entities;
using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.ConfigReaders
{
    public class FileConfigReader : IConfigReader
    {
        private readonly string _configFilePath;

        public FileConfigReader(string configFilePath)
        {
            _configFilePath = System.Environment.ExpandEnvironmentVariables(configFilePath);
        }

        public MediaDashboardConfig ReadConfig()
        {
            var config = File.ReadAllText(_configFilePath);

            return JsonConvert.DeserializeObject<MediaDashboardConfig>(config);
        }
    }
}