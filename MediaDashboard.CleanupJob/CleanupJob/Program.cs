using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Persistence.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CleanupJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            Trace.TraceInformation("Starting cleanup job.");
            var start = DateTime.UtcNow;
            var tasks = GetCleanupTasks().ToList();
            Trace.TraceInformation("Found {0} tasks...", tasks.Count);
            try
            {
                Task.WhenAll(tasks).Wait();
            }
            catch(AggregateException ae)
            {
                Trace.TraceError("Cleanup failed! Error:{0}", ae);
            }
            finally
            {
                var time = DateTime.UtcNow - start;
                Trace.TraceInformation("Cleanup time taken:{0}", time);
            }
        }

        private static IEnumerable<Task> GetCleanupTasks()
        {
            var contentProviders = App.Config.Content.ContentProviders;
            foreach (var contentProvider in contentProviders)
            {
                var mediaServiceSets = contentProvider.MediaServicesSets;
                foreach (var set in mediaServiceSets)
                {
                    yield return Task.Run(() => CleanupDatabase(set));

                    foreach (var mediaService in set.MediaServicesAccounts)
                    {
                        if(mediaService.TelemetryStorage != null)
                        {
                            yield return Task.Run(() => CleanupTelemetry(mediaService.TelemetryStorage));
                        }
                    }
                }
            }
        }

        const string TablePrefix = "TelemetryMetrics";

        static readonly int SqlRetentionPeriod = (App.Config.Sys.Maintenance?.SqlRentionPeriod) ?? MaintenanceConfig.DefaultRentionPeriodInDays;

        static readonly int TableStorageRetentionPeriod = (App.Config.Sys.Maintenance?.TableStorageRetentionPeriod) ?? MaintenanceConfig.DefaultRentionPeriodInDays;

        private static bool DeleteTable(CloudTable table)
        {
            var date = table.Name.Substring(TablePrefix.Length);
            // date is of format yyyymmdd
            if(date.Length != 8)
            {
                return false;
            }
            var year = Convert.ToInt32(date.Substring(0, 4));
            var month = Convert.ToInt32(date.Substring(4, 2));
            var day = Convert.ToInt32(date.Substring(6, 2));
            var tableDate  = new DateTime(year, month, day);
            var timeSpan = DateTime.UtcNow - tableDate;
            Trace.TraceInformation("Table {0} is {1} days old", table.Name, timeSpan.Days);
            if (timeSpan.Days > TableStorageRetentionPeriod)
            {
                Trace.TraceInformation("Table {0} is older than {1} days", table.Name, TableStorageRetentionPeriod);
                return true;
            }
            return false;
        }

        private static void CleanupTelemetry(StorageAccountConfig  account)
        {
            Trace.TraceInformation("Cleaning up telemetry in account:{0}", account.AccountName);
            var credentials = new StorageCredentials(account.AccountName, account.AccountKey);
            var storage = new CloudStorageAccount(credentials, useHttps:true);
            var tableClient = storage.CreateCloudTableClient();

            var tables = tableClient.ListTables("Telemetry");
            foreach(var table in tables)
            {
                if (DeleteTable(table))
                {
                    Trace.TraceWarning("Deleting table: {0}", table.Name);
                    table.Delete();
                }
                else
                {
                    Trace.TraceInformation("Ignoring table:{0} as per retention policy", table.Name);
                }
            }
            Trace.TraceInformation("Cleaned telemety for account:{0}", account.AccountName);
        }

        private static void CleanupDatabase(MediaServicesSetConfig mediaServicesSet)
        {
            var rententionPeriod = TimeSpan.FromDays(SqlRetentionPeriod);
            var earliestDate = DateTime.UtcNow - rententionPeriod;
            Trace.TraceInformation("Earliest date before which the record will be removed: {0}", earliestDate);
            foreach(var databaseConfig in mediaServicesSet.DataStorageConnections)
            {
                Trace.TraceInformation(
                    "Cleaning database account on Server:{0} Accont:{1} Catalog:{2}",
                    databaseConfig.AzureServer,
                    databaseConfig.AcctName,
                    databaseConfig.InitialCatalog);
            }

            // Clean channel and origin alerts older than 30 days.
            var dataAccess = new AzureDataAccess(mediaServicesSet.DataStorageConnections);
            int removed = dataAccess.CleanOutOldData(earliestDate);
            Trace.TraceInformation("Total of {0} records removed on {1} ", removed, DateTime.UtcNow);

            #region Not Used
            //dataAccess.ExecuteWriteQuery( dataContext =>
            //{
            //    foreach (AlertType at in dataContext.AlertTypes)
            //    {
            //        //start with GlobalAlerts
            //        var records = dataContext.Alerts.Where(alert => (alert.AlertDate < earliestDate.Date) &&
            //        alert.AlertTypeID == at.AlertTypeID).ToList();
            //        switch (at.AlertTypeID)
            //        {
            //            case 1:
            //                Trace.TraceInformation("Removing {0} records from Global Alerts...", records.Count);
            //                break;
            //            case 2:
            //                Trace.TraceInformation("Removing {0} records from Channel Alerts...", records.Count);
            //                break;
            //            case 3:
            //                Trace.TraceInformation("Removing {0} records from Origin Alerts...", records.Count);
            //                break;
            //            case 4:
            //                Trace.TraceInformation("Removing {0} records from Archive Alerts...", records.Count);
            //                break;
            //        }
            //        dataContext.Alerts.RemoveRange(records);
            //        dataContext.SaveChanges();
            //    }

            //});

            //dataAccess.ExecuteWriteQuery(dataContext =>
            //{
            //    var records = dataContext.Alerts.Where(alert => (alert.AlertDate < earliestDate.Date) &&
            //    alert.AlertTypeID == 2).ToList();

            //    dataContext.Alerts.RemoveRange(records);
            //    dataContext.SaveChanges();
            //});

            //dataAccess.ExecuteWriteQuery(dataContext =>
            //{
            //    var records = dataContext.Alerts.Where(alert => (alert.AlertDate < earliestDate.Date) &&
            //    alert.AlertTypeID == 3).ToList();

            //    dataContext.Alerts.RemoveRange(records);
            //    dataContext.SaveChanges();
            //});

            //dataAccess.ExecuteWriteQuery(dataContext =>
            //{
            //    var records = dataContext.Alerts.Where(alert => (alert.AlertDate < earliestDate.Date) &&
            //    alert.AlertTypeID == 4).ToList();

            //    dataContext.Alerts.RemoveRange(records);
            //    dataContext.SaveChanges();
            //});
            #endregion
        }

    }
}
