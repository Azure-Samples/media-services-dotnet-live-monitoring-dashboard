using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MediaDashboard.Common;
using MediaDashboard.Web.Api.Controllers;
using Microsoft.WindowsAzure.MediaServices.Client;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;

namespace MediaDashboard.Operations.Api.Test
{
    [TestFixture]
    class TelemetryTests
    {
        [Test]
        public void GetChannelTelemetryTest()
        {
            List<AventusTelemetry> telems = new List<AventusTelemetry>();
            var mds = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new ChannelTelemetryController();
            var ctxt = mds.GetContext();
            foreach (var channel in ctxt.Channels)
            {
                if (channel.EncodingType != ChannelEncodingType.None && channel.State == ChannelState.Running)
                {
                    string channelID = channel.Id.Replace("nb:chid:UUID:", string.Empty);
                    var telemetry = controller.Get(channelID) as AventusTelemetry;
                    
                    if(telemetry!=null)
                        telems.Add(telemetry);
                }
            }
            Assert.IsTrue(telems.Count > 0);
        }

        [Test]
        public void GetChannelTelmetryByIdTest()
        {
            var mds = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new ChannelTelemetryController();
            var context = mds.GetContext();

            var channels = context.Channels.ToList();
            var channel = channels.Where(ch => ch.EncodingType != ChannelEncodingType.None && ch.State == ChannelState.Running).FirstOrDefault();
            if (channel != null)
            {
                var telemetry = controller.Get(channel.Id.NimbusIdToRawGuid());
                Assert.IsNotNull(telemetry);
            }
            else
            {
                Assert.Inconclusive("Channel may not be present");
            }

            
        }

        [Test]
        public void TestIngestMetrics()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var context = account.GetContext();
            var channels = context.Channels.ToList();
            foreach (var channel in channels.Where(channel => channel.State == ChannelState.Running))
            {
                var controller = new IngestTelemetryController();
                var metrics = controller.Get(account.AccountName, channel.Id.NimbusIdToRawGuid());
                Assert.IsNotNull(metrics);
            }
        }

        [Test]
        public void TestArchiveMetrics()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var channels = context.Channels.ToList();
            foreach (var channel in channels.Where(channel => channel.State == ChannelState.Running))
            {
                // check if there are programs running.
                var programs = channel.Programs.ToList();
                if(programs.Any(program => program.State == ProgramState.Running))
                {
                    var controller = new ArchiveTelemetryController();
                    var metrics = controller.Get(account.AccountName, channel.Id.NimbusIdToRawGuid());
                    Assert.IsNotNull(metrics);
                }
            }
        }


        [Test]
        public void TestOriginMetrics()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var context = account.GetContext();
            var origins = context.StreamingEndpoints.ToList();
            foreach(var origin in origins.Where(origin => origin.State == StreamingEndpointState.Running))
            {
                var controller = new OriginTelemetryController();
                var metrics = controller.GetMetrics(account.AccountName, origin.Id.NimbusIdToRawGuid());
                Assert.IsNotNull(metrics);
            }
        }

    }
}
