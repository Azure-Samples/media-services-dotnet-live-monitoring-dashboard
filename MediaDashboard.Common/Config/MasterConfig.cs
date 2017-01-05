using Microsoft.Azure;

namespace MediaDashboard.Common.Config
{
    public static class MasterConfig
    {
        //Gest config file location either from CloudServiceConfiguration or web/app.config
        private static string STORAGECONFIGKEY = "MediaDashboard.ConfigurationFileAccount";
        public static string ConfigStorageConnectionString
        {
            
            get
            {
                string configLocation = CloudConfigurationManager.GetSetting(STORAGECONFIGKEY);
                return configLocation;
            }
        }

        public static string ConfigContainerName
        {
            get { return "config"; }
        }

        public static string ConfigBlobName
        {
            get { return "mediadashboardconfig.json"; }
        }
    }
}
