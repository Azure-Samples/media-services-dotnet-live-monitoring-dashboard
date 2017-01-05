using MediaDashboard.Common;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaDashboard.Ingest
{
    public class MonitoringController
    {
        public void Start()
        {
            StartAsync().Wait();
        }

        public async Task StartAsync()
        {
            await Task.WhenAll(GetMonitoringTasks());
        }

        private IEnumerable<Task> GetMonitoringTasks()
        {
            var contentProviders = App.Config.Content.ContentProviders;
            foreach (var contentProvider in contentProviders)
            {
                var mediaServiceSets = contentProvider.MediaServicesSets;
                foreach (var set in mediaServiceSets)
                {
                    var mediaServices = set.MediaServicesAccounts;
                    foreach (var mediaService in set.MediaServicesAccounts)
                    {
                        Trace.TraceInformation("Collecting information for {0}", mediaService.AccountName);
                        var worker = new MonitoringWorker(new AzureMediaService(mediaService), set.DataStorageConnections);
                        yield return worker.RunAsync();
                    }
                }
            }
        }
    }
}
