using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaDashboard.Ingest
{

    /// <summary>
    /// One instance of this object for each media service being monitored
    /// </summary>
    public class AzureMediaService
    {

        public MediaServicesCredentials Credentials { get; private set; }

        public CloudMediaContext CloudContext { get; private set; }

        public string Id { get; set; }

        public string Name
        {
            get { return Credentials.ClientId; }
        }

        public MediaServicesAccountConfig Config { get; private set; }

        public AzureMediaService(string accountName, string accountKey)
        {
            Credentials = new MediaServicesCredentials(accountName, accountKey);
            CloudContext = new CloudMediaContext(Credentials);
        }

        public AzureMediaService(MediaServicesAccountConfig config)
        {
            Config = config;
            CloudContext = Config.GetContext();
            Credentials = new MediaServicesCredentials(Config.AccountName, Config.AccountKey);
        }
    }
}
