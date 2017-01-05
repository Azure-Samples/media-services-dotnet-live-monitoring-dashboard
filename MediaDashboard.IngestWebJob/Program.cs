using MediaDashboard.Ingest;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace MediaDashboard.IngestWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            const int _waitTime = 30000;

            Trace.TraceInformation("IngestWebJob entry point called.");
            while (true)
            {
                try
                {

                    Trace.TraceInformation("Monitoring instance started");
                    var _monitor = new MonitoringController();
                    var start = DateTime.Now;
                    _monitor.Start();
                    Trace.TraceInformation("Total time for processing: {0}", DateTime.Now.Subtract(start));
                }
                catch (AggregateException ae)
                {
                    ae = ae.Flatten();
                    Trace.TraceError("Aggregate exception: {0}", ae);
                    Trace.TraceError("source: {0} exception: {1} stacktrace: {2}", ae.Source, ae.Message, ae.StackTrace);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("System Exception: {0}", ex);
                    Trace.TraceError("source: {0} exception: {1} stacktrace: {2}", ex.Source, ex.Message, ex.StackTrace);
                }
                finally
                {
                    Thread.Sleep(_waitTime);
                }
            }
        }
    }
}
