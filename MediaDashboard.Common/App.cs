using MediaDashboard.Common.Config;
using MediaDashboard.Common.Config.Entities;

namespace MediaDashboard.Common
{
    public static class App
    {
        private static object _lockpad = new object();
        private static MediaDashboardConfig _mediaDashboardConfig;

        public static MediaDashboardConfig Config
        {
            get
            {
                if (_mediaDashboardConfig == null)

                {
                    lock (_lockpad)
                    {
                        if (_mediaDashboardConfig == null)
                            ReLoadConfig();
                    }
                }

                return _mediaDashboardConfig;
            }
        }

        private static void ReLoadConfig()
        {
            var configReader = ConfigSelector.Get();
            _mediaDashboardConfig = configReader.ReadConfig();
        }
    }
}