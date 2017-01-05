using MediaDashboard.Common;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Web.Api.Controllers;
using MediaDashboard.Web.Api.Models;
using Microsoft.WindowsAzure.MediaServices.Client;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace MediaDashboard.Operations.Api.Test
{
    [TestFixture]
    class AlertsControllerTests
    {

        [Test]
        public void GetChannelAlertsTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var channels = context.Channels.ToList();
            var runningChannels = channels.Where(c => c.State == ChannelState.Running);
            var controller = new ChannelAlertsController();

            foreach(var channel in runningChannels)
            {
                var alerts = controller.Get(account.AccountName, channel.Id.NimbusIdToRawGuid(), null).ToList();
                Trace.TraceInformation("Channe:{0}, Alerts:{1}", channel.Id, alerts.Count);
            }
        }

        [Test,Ignore("No Longer Applicable")]
        public void GetChannelAlertsWithTimeRangeTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var channels = context.Channels.ToList();
            var runningChannels = channels.Where(c => c.State == ChannelState.Running);
            var controller = new ChannelAlertsController();

            var query = new AlertsQuery
            {
                EndTime = DateTime.UtcNow,
                StartTime = DateTime.UtcNow - TimeSpan.FromHours(1),
                MetricTypes = new[] { MetricType.Archive, MetricType.Encoding, MetricType.Ingest},
                StatusLevels = new [] { HealthStatus.Warning, HealthStatus.Critical}                
            };
            foreach (var channel in runningChannels)
            {
                var alerts = controller.Get(account.AccountName, channel.Id.NimbusIdToRawGuid(), query).ToList();
                Trace.TraceInformation("Channe:{0}, Alerts:{1}", channel.Id, alerts.Count);
            }
        }


        [Test]
        public void GetOriginAlertsTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var origins = context.StreamingEndpoints.ToList();
            var runningOrigins = origins.Where(s => s.State == StreamingEndpointState.Running);
            var controller = new OriginAlertsController();

            var query = new AlertsQuery
            {
                EndTime = DateTime.UtcNow,
                StartTime = DateTime.UtcNow - TimeSpan.FromHours(1),
                StatusLevels = new[] { HealthStatus.Warning, HealthStatus.Critical }
            };
            foreach (var origin in runningOrigins)
            {
                var alerts = controller.Get(account.AccountName, origin.Id.NimbusIdToRawGuid(), query).ToList();
                Trace.TraceInformation("Channe:{0}, Alerts:{1}", origin.Id, alerts.Count);
            }
        }

        [Test, Ignore("No Longer Applicable")]
        public void GetOriginAlertsWithTimeRangeTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var origins = context.StreamingEndpoints.ToList();
            var runningOrigins = origins.Where(s => s.State == StreamingEndpointState.Running);
            var controller = new OriginAlertsController();

            foreach (var origin in runningOrigins)
            {
                var alerts = controller.Get(account.AccountName, origin.Id.NimbusIdToRawGuid(), null).ToList();
                Trace.TraceInformation("Channe:{0}, Alerts:{1}", origin.Id, alerts.Count);
            }
        }
    }
}
