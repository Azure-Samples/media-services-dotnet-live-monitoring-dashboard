using MediaDashboard.Common;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Web.Api.Controllers;
using MediaDashboard.Web.Api.Models;
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
    public class ProgramDataTests
    {
        [Test]
        public void GetProgramTest()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var context = account.GetContext();
            var program = context.Programs.FirstOrDefault();
            Assert.IsNotNull(program, "No programs found!");
            ProgramsController controller = new ProgramsController();
            var programDetails = controller.Get(account.AccountName, program.Id.NimbusIdToRawGuid());
            Assert.IsNotNull(programDetails, "Program cannot be null");
            Assert.AreEqual(program.Name, programDetails.Name);
            Assert.AreEqual(program.Id, programDetails.Id.GuidToProgramId());
            Assert.AreEqual(program.Description, programDetails.Description);
            Assert.AreEqual(program.State.ToString(), programDetails.State);
            Assert.AreEqual(program.ChannelId, programDetails.ChannelId);
        }

        [Test]
        public void GetProgramsForChannelTest()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var context = account.GetContext();
            var channel = context.Channels.FirstOrDefault();
            Assert.IsNotNull(channel, "No programs found!");
            ProgramsController controller = new ProgramsController();
            var programDetails = controller.GetProgramsForChannel(account.AccountName, channel.Id.NimbusIdToRawGuid());
            Assert.IsNotNull(programDetails);
        }

        [Test]
        public void GetAllProgramsTest()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var context = account.GetContext();
            var programs = context.Programs.ToList();
            ProgramsController controller = new ProgramsController();
            var programDetails = controller.GetAllPrograms(account.AccountName);
            Assert.IsNotNull(programDetails);
            Assert.AreEqual(programs.Count, programDetails.Count);
        }

        [Test]
        public void StartProgramTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new ProgramsController();
            var programs = context.Programs.ToList();
            var program = programs.FirstOrDefault(p => p.State == ProgramState.Stopped);
            if (program != null)
            {
                controller.Start(account.AccountName, program.Id.NimbusIdToRawGuid());
                program = context.Programs.Where(c => c.Id == program.Id).FirstOrDefault();
                Assert.AreEqual(ProgramState.Starting, program.State);
            }
            //trying to start a running program fails.
            program = programs.FirstOrDefault(p => p.State != ProgramState.Stopped);
            if (program != null)
            {
                try
                {
                    controller.Start(account.AccountName, program.Id.NimbusIdToRawGuid());
                }
                catch (HttpResponseException he)
                {
                    Assert.AreEqual(HttpStatusCode.PreconditionFailed, he.Response.StatusCode);
                }
            }
        }

        [Test]
        public void StopProgramTest()
        {
            var account = App.Config.GetDefaultAccount();
            var context = account.GetContext();
            var controller = new ProgramsController();
            var programs = context.Programs.ToList();
            var program = programs.FirstOrDefault(p => p.State == ProgramState.Running);
            if (program != null)
            {
                controller.Stop(account.AccountName, program.Id.NimbusIdToRawGuid());
                program = context.Programs.Where(c => c.Id == program.Id).FirstOrDefault();
                Assert.AreEqual(ProgramState.Stopping, program.State);
            }
            //trying to stop a stopped program fails.
            program = programs.FirstOrDefault(p => p.State != ProgramState.Running);
            if (program != null)
            {
                try
                {
                    controller.Stop(account.AccountName, program.Id.NimbusIdToRawGuid());
                }
                catch (HttpResponseException he)
                {
                    Assert.AreEqual(HttpStatusCode.PreconditionFailed, he.Response.StatusCode);
                }
            }
        }

        [Test]
        public void ProgramCRUDTest()
        {
            var account = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
            var controller = new ProgramsController();
            var context = account.GetContext();

            var channel = context.Channels.FirstOrDefault();
            int programCount = context.Programs.Count();
            var name = string.Format("New-Program-{0}", DateTime.UtcNow.ToOADate().ToString().Replace(".", "-"));

            var settings = new ProgramSettings
            {
                Name = name,
                Description = name,
                ArchiveWindowLength = TimeSpan.FromHours(1)
            };

            controller.Create(account.AccountName, channel.Id.NimbusIdToRawGuid(), settings);
            Assert.AreEqual(programCount + 1, context.Programs.Count());

            var program = context.Programs.ToList().First(o => o.Name == name);

            //update the channel.
            var updateSettings = new ProgramUpdateSettings
            {
                Description = "SomeDescription"
            };
            controller.Update(account.AccountName, program.Id.NimbusIdToRawGuid(), updateSettings);

            // create new context to avoid cache issue.
            context = account.GetContext();
            program = context.Programs.Where(p => p.Id == program.Id).FirstOrDefault();
            Assert.IsNotNull(program);

            Assert.AreEqual(updateSettings.Description, program.Description);

            controller.Delete(account.AccountName, program.Id.NimbusIdToRawGuid());
            Assert.AreEqual(programCount, context.Programs.Count());
        }

    }
}
