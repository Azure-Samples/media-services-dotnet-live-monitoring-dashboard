using MediaDashboard.Common;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Web.Api.Controllers;
using Models = MediaDashboard.Web.Api.Models;
using Microsoft.WindowsAzure.MediaServices.Client;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Test
{
    [TestFixture]
    class ChannelControllerTests
    {

        [Test]
        public void GetChannelControllerTest()
        {
            var mds=App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new ChannelsController();
            Assert.IsNotNull(controller.DataAccess);
            Assert.IsInstanceOf<ChannelsController>(controller);
        }

        [Test]
        public void GetChannelsTest()
        {
            var mds = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new ChannelsController();

            try
            {
                var channels = controller.GetAllChannels(mds.AccountName);
                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() >= 0);
            }
            catch (Exception ex)
            {
                Assert.Fail("Excetion: {0}", ex);
            }
        }

        [Test]
        public void ChannelCRUDTest()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new ChannelsController();
            var context = account.GetContext();
            int chnlCount= context.Channels.Count();
            var name = string.Format("New-Channel-{0}", DateTime.UtcNow.ToOADate().ToString().Replace(".", "-"));
            var settings = new Models.ChannelSettings
            {
                Name = name
            };
            var operation = controller.Create(account.AccountName, settings);
            while(operation.State == OperationState.InProgress)
            {
                Thread.Sleep(10000);
                operation = context.Operations.GetOperation(operation.Id);
            }
            Assert.AreEqual(chnlCount + 1, context.Channels.Count());

            var channel = context.Channels.ToList().First(c => c.Name == name);

            var range = new Models.IPRange
            {
                SubnetPrefixLength = 32,
                Address = "127.0.0.1"
            };

            //update the channel.
            var updateSettings = new Models.ChannelUpdateSettings
            {
                IngestAllowList = new Models.IPRange[] { range },
                PreviewAllowList = new Models.IPRange[] { range },
                Description = "SomeDescription"
            };
            operation = controller.Update(account.AccountName, channel.Id.NimbusIdToRawGuid(), updateSettings);
            while (operation.State == OperationState.InProgress)
            {
                Thread.Sleep(10000);
                operation = context.Operations.GetOperation(operation.Id);
            }

            // create new context to avoid cache issue.
            context = account.GetContext();
            channel = context.Channels.Where(c => c.Id == channel.Id).FirstOrDefault();
            Assert.IsNotNull(channel);

            // verify ingest.
            Assert.AreEqual(1, channel.Input.AccessControl.IPAllowList.Count);
            Assert.AreEqual("Range0", channel.Input.AccessControl.IPAllowList[0].Name);
            Assert.AreEqual(range.SubnetPrefixLength, channel.Input.AccessControl.IPAllowList[0].SubnetPrefixLength);
            Assert.AreEqual(range.Address, channel.Input.AccessControl.IPAllowList[0].Address);

            // verify preview.
            Assert.AreEqual(1, channel.Preview.AccessControl.IPAllowList.Count);
            Assert.AreEqual("Range0", channel.Preview.AccessControl.IPAllowList[0].Name);
            Assert.AreEqual(range.SubnetPrefixLength, channel.Preview.AccessControl.IPAllowList[0].SubnetPrefixLength);
            Assert.AreEqual(range.Address, channel.Preview.AccessControl.IPAllowList[0].Address);

            operation = controller.Delete(account.AccountName, channel.Id.NimbusIdToRawGuid());
            while (operation.State == OperationState.InProgress)
            {
                Thread.Sleep(10000);
                operation = context.Operations.GetOperation(operation.Id);
            }
            Assert.AreEqual(chnlCount, context.Channels.Count());
        }

        [Test]
        public void GetChannelByNameTest()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new ChannelsController();

            var channel = account.GetContext().Channels.FirstOrDefault();
            Assert.IsNotNull(channel, "Channel not created in cloud context");
            var channelDetails = controller.GetChannelByName(channel.Name);
            Assert.IsNotNull(channelDetails, "Channel data not saved to database");
            Assert.AreEqual(channel.Name, channelDetails.NameShort);            
        }

        [Test]
        public void GetChannelProgramsTest()
        {
            var account = App.Config.GetDefaultAccount();
            var controller = new ChannelsController();
            var context = account.GetContext();
            Assert.IsNotNull(context);

            MediaChannel chlContent =null;
            foreach (var channel in context.Channels)
            {
                int programCount = channel.Programs.Count();
                if (programCount > 0)
                {
                    chlContent = controller.GetChannelById(account.AccountName, channel.Id.NimbusIdToRawGuid());
                    Assert.IsNotNull(chlContent, "Context not Accessible");
                    break;
                }
            }
        }

        [Test]
        public void GetChannelByLongChannelIdTest()
        {
            var ctrlr = new ChannelsController();
            string acct = "dashboardbl2"; //channels/;
            string id = "0405bb3a-fa37-4e26-ad49-fb1473b48093";

            var result = ctrlr.GetChannelById(acct, id);
            Assert.IsNotNull(result);
        }

        [Test]
        public void StartChannelTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new ChannelsController();
            var channels = context.Channels.ToList();
            var channel = channels.FirstOrDefault(c => c.State == ChannelState.Stopped);
            if (channel != null)
            {
                controller.Start(account.AccountName, channel.Id.NimbusIdToRawGuid());
                channel = context.Channels.Where(c => c.Id == channel.Id).FirstOrDefault();
                Assert.AreEqual(ChannelState.Starting, channel.State);
            }
            //trying to start a running channel fails.
            channel = channels.FirstOrDefault(c => c.State != ChannelState.Stopped);
            if (channel != null)
            {
                try
                {
                    controller.Start(account.AccountName, channel.Id.NimbusIdToRawGuid());
                }
                catch(HttpResponseException he)
                {
                    Assert.AreEqual(HttpStatusCode.PreconditionFailed, he.Response.StatusCode);
                }
            }
        }

        [Test]
        public void StopChannelTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new ChannelsController();
            var channels = context.Channels.ToList();
            var channel = channels.FirstOrDefault(c => c.State == ChannelState.Running);
            if (channel != null)
            {
                controller.Stop(account.AccountName, channel.Id.NimbusIdToRawGuid());
                channel = context.Channels.Where(c => c.Id == channel.Id).FirstOrDefault();
                Assert.AreEqual(ChannelState.Stopping, channel.State);
            }
            //trying to start a running channel fails.
            channel = channels.FirstOrDefault(c => c.State != ChannelState.Stopped);
            if (channel != null)
            {
                try
                {
                    controller.Stop(account.AccountName, channel.Id.NimbusIdToRawGuid());
                }
                catch (HttpResponseException he)
                {
                    Assert.AreEqual(HttpStatusCode.PreconditionFailed, he.Response.StatusCode);
                }
            }
        }

        [Test]
        public void InsertSlateTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new ChannelsController();
            var channels = context.Channels.ToList();
            var channel = channels.FirstOrDefault(c => c.State == ChannelState.Running && c.EncodingType != ChannelEncodingType.None);
            if (channel != null)
            {
                var settings = new Models.SlateSettings
                {
                    Duration = 30
                };
                var operation = controller.Slate(account.AccountName, channel.Id.NimbusIdToRawGuid(), settings);
            }
        }

        [Test]
        public void InsertAdTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new ChannelsController();
            var channels = context.Channels.ToList();
            var channel = channels.FirstOrDefault(c => c.State == ChannelState.Running && c.EncodingType != ChannelEncodingType.None);
            if (channel != null)
            {
                var settings = new Models.SlateSettings
                {
                    Duration = 30,
                    ShowSlate = true,
                    CueId = (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds
                };
                var operation = controller.AdMarker(account.AccountName, channel.Id.NimbusIdToRawGuid(), settings);
            }
        }
    }
}
