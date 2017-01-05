using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using MediaDashboard.Ingest;

namespace MediaDashboard.IngestWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        const int _waitTime = 30000;
        MonitoringWorker _worker;
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("MediaDashboard.IngestWorker entry point called\r\n");
            MonitoringController _monitor = null;
            while (true)
                {
                    try
                    {
                
                            _monitor = new MonitoringController();
                            _monitor.Start();

                            Debug.Write("Monitoring instance started", "information");
                            
                    }
                    catch (Exception _ex)
                    {
                        Debug.Write(string.Format("source: {0} exception: {1} stacktrace: {2}", _ex.Source, _ex.Message, _ex.StackTrace), "warning");
                    }
                    finally
                    {
                        Thread.Sleep(_waitTime);
                    }
                }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
