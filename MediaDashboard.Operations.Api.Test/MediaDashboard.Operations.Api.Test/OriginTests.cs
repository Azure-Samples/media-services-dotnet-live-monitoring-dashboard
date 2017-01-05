using MediaDashboard.Common;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Web.Api.Controllers;
using Models =MediaDashboard.Web.Api.Models;
using Microsoft.WindowsAzure.MediaServices.Client;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Test
{
    [TestFixture]
    class OriginTests
    {
        [Test]
        public void GetAllOriginsTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var endpoints = context.StreamingEndpoints.ToList().OrderBy(e => e.Name).ToList();
            var controller = new OriginsController();
            var origins = controller.GetOrigins(account.AccountName);
            Assert.AreEqual(endpoints.Count, origins.Count);
            for(var i = 0; i < origins.Count; ++i)
            {
                Compare(endpoints[i], origins[i]);
            }
        }

        private void Compare(IStreamingEndpoint endpoint, MediaOrigin origin)
        {
            Assert.AreEqual(endpoint.Id.Substring(12), origin.Id);
            Assert.AreEqual(endpoint.Name, origin.Name);
            Assert.AreEqual(endpoint.HostName, origin.HostName);
            Assert.AreEqual(endpoint.Created, origin.Created);
            Assert.AreEqual(endpoint.LastModified, origin.LastModified);
            Assert.AreEqual(endpoint.ScaleUnits, origin.ReservedUnits);
            Assert.AreEqual(endpoint.State.ToString(), origin.State);
            Assert.AreEqual(endpoint.CacheControl?.MaxAge, origin.MaxAge);
            if(endpoint.AccessControl != null)
            {
                var ipAllowList = string.Join(";",
                    endpoint.AccessControl.IPAllowList.Select(iprange => string.Format("{0}/{1}", iprange.Address, iprange.SubnetPrefixLength)));
                Assert.AreEqual(ipAllowList, origin.IPAllowList);
            }
        }

        [Test]
        public void GetOriginByIdTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var endpoint = context.StreamingEndpoints.ToList()
                .FirstOrDefault();
            Assert.IsNotNull(endpoint, "No streaming endpoint present");
            var controller = new OriginsController();
            var origin = controller.GetOriginById(account.AccountName, endpoint.Id.Substring(12));
            Compare(endpoint, origin);
        }

        [Test]
        public void StartOriginTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new OriginsController();
            var origins = context.StreamingEndpoints.ToList();
            var origin = origins.FirstOrDefault(e => e.State == StreamingEndpointState.Stopped);
            if (origin != null)
            {
                controller.Start(account.AccountName, origin.Id.NimbusIdToRawGuid());
                origin = context.StreamingEndpoints.Where(c => c.Id == origin.Id).FirstOrDefault();
                Assert.AreEqual(StreamingEndpointState.Starting, origin.State);
            }
            
            //trying to start a running origin fails.
            origin = origins.FirstOrDefault(e => e.State != StreamingEndpointState.Stopped);
            if (origin != null)
            {
                try
                {
                    controller.Start(account.AccountName, origin.Id.NimbusIdToRawGuid());
                }
                catch (HttpResponseException he)
                {
                    Assert.AreEqual(HttpStatusCode.PreconditionFailed, he.Response.StatusCode);
                }
            }
        }

        [Test]
        public void StopOriginTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new OriginsController();
            var origins = context.StreamingEndpoints.ToList();
            var origin = origins.FirstOrDefault(p => p.State == StreamingEndpointState.Running);
            if (origin != null)
            {
                controller.Stop(account.AccountName, origin.Id.NimbusIdToRawGuid());
                origin = context.StreamingEndpoints.Where(c => c.Id == origin.Id).FirstOrDefault();
                Assert.AreEqual(StreamingEndpointState.Stopping, origin.State);
            }
            //trying to stop a stopped origin fails.
            origin = origins.FirstOrDefault(p => p.State != StreamingEndpointState.Running);
            if (origin != null)
            {
                try
                {
                    controller.Stop(account.AccountName, origin.Id.NimbusIdToRawGuid());
                }
                catch (HttpResponseException he)
                {
                    Assert.AreEqual(HttpStatusCode.PreconditionFailed, he.Response.StatusCode);
                }
            }
        }


        [Test]
        public void OriginCRUDTest()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new OriginsController();
            var context = account.GetContext();
            int originCount = context.StreamingEndpoints.Count();
            var name = string.Format("New-Origin-{0}", DateTime.UtcNow.ToOADate().ToString().Replace(".", "-"));
            var settings = new Models.OriginSettings
            {
                Name = name
            };
            var operation = controller.Create(account.AccountName, settings);
            while (operation.State == OperationState.InProgress)
            {
                Thread.Sleep(10000);
                operation = context.Operations.GetOperation(operation.Id);
            }
            Assert.AreEqual(originCount + 1, context.StreamingEndpoints.Count());

            var origin = context.StreamingEndpoints.ToList().First(o => o.Name == name);

            var range = new Models.IPRange
            {
                SubnetPrefixLength = 32,
                Address = "127.0.0.1"
            };

            //update the channel.
            var updateSettings = new Models.OriginUpdateSettings
            {
                AllowList = new Models.IPRange[] { range },
                Description = "SomeDescription"
            };
            operation = controller.Update(account.AccountName, origin.Id.NimbusIdToRawGuid(), updateSettings);
            while (operation.State == OperationState.InProgress)
            {
                Thread.Sleep(10000);
                operation = context.Operations.GetOperation(operation.Id);
            }

            // create new context to avoid cache issue.
            context = account.GetContext();
            origin = context.StreamingEndpoints.Where(o => o.Id == origin.Id).FirstOrDefault();
            Assert.IsNotNull(origin);

            Assert.AreEqual(updateSettings.Description, origin.Description);

            // verify access control.
            Assert.AreEqual(1, origin.AccessControl.IPAllowList.Count);
            Assert.AreEqual("Range0", origin.AccessControl.IPAllowList[0].Name);
            Assert.AreEqual(range.SubnetPrefixLength, origin.AccessControl.IPAllowList[0].SubnetPrefixLength);
            Assert.AreEqual(range.Address, origin.AccessControl.IPAllowList[0].Address);

            operation = controller.Delete(account.AccountName, origin.Id.NimbusIdToRawGuid());
            while (operation.State == OperationState.InProgress)
            {
                Thread.Sleep(10000);
                operation = context.Operations.GetOperation(operation.Id);
            }
            Assert.AreEqual(originCount, context.StreamingEndpoints.Count());
        }
    }
}
