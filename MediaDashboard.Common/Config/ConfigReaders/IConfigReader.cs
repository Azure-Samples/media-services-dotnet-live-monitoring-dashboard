using MediaDashboard.Common.Config.Entities;

namespace MediaDashboard.Common.Config.ConfigReaders
{
    interface IConfigReader
    {
        MediaDashboardConfig ReadConfig();
    }
}