using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Web.Api.Models;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace MediaDashboard.Web.Api.Controllers
{
    public class ProgramsController : ControllerBase
    {
        public List<MediaProgram> GetAllPrograms(string account)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            return GetAllPrograms(accountConfig);
        }

        [HttpGet]
        [Route("api/Accounts/{account}/Channels/{channelId}/Programs")]
        public List<MediaProgram> GetProgramsForChannel(string account, string channelId)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            var context = accountConfig.GetContext();
            string chId = channelId.GuidToChannelId();
            var channel = context.Channels.Where(ch => ch.Id == chId).FirstOrDefault();
            if (channel == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            return channel.Programs.AsEnumerable().Select(GetProgramDetails).ToList();
        }

        [HttpGet]
        [Route("api/Accounts/{account}/Programs/{id}/Urls")]
        public ProgramUrls GetUrls(string account, string id, [FromUri] string originHostName = null)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var context = accountConfig.GetContext();
            string prgId = id.GuidToProgramId();
            var program = context.Programs.Where(pr => pr.Id == prgId).FirstOrDefault();
            if (program == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var urls = program.GetStreamingUrls();
            if (urls != null && originHostName != null)
            {
                urls.UpdateStreamingUrls(originHostName);
            }
            return urls;
        }

        public List<MediaProgram> GetAllPrograms(MediaServicesAccountConfig accountConfig)
        {
            var context = accountConfig.GetContext();
            var programs = context.Programs.ToList();
            var details = programs.Select(GetProgramDetails).ToList();
            return details;
        }

        public MediaProgram Get(string account, string id)
        {
            var program = MapProgram(account, id);
            return GetProgramDetails(program);
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IOperation Start(string account, string id)
        {
            var program = MapProgram(account, id);
            if(program.State != ProgramState.Stopped)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.PreconditionFailed);
            }
            var operation = program.SendStartOperation();
            Trace.TraceInformation("Starting program. Operation {0} on Program {1}", operation.Id, operation.TargetEntityId);
            return operation;
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IOperation Stop(string account, string id)
        {
            var program = MapProgram(account, id);
            if (program.State != ProgramState.Running)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.PreconditionFailed);
            }
            var operation = program.SendStopOperation();
            Trace.TraceInformation("Stopping program. Operation {0} on Program {1}", operation.Id, operation.TargetEntityId);
            return operation;
        }

        [HttpDelete]
        [Authorize(Roles = Role.Administrator)]
        public void Delete(string account, string id)
        {
            var program = MapProgram(account, id);
            if (program.State != ProgramState.Stopped)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.PreconditionFailed);
            }
            program.Delete();
        }


        [HttpPost]
        [Authorize(Roles = Role.Administrator)]
        [Route("api/Accounts/{account}/Channels/{id}/Programs")]
        public MediaProgram Create(string account, string id, [FromBody] ProgramSettings programSettings)
        {
            if(programSettings == null || programSettings.Name == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var context = accountConfig.GetContext();
            var channelId = id.GuidToChannelId();
            var channel = context.Channels.Where(c => c.Id == channelId).FirstOrDefault();
            if(channel == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var programOptions = programSettings.GetCreationOptions();
            if(programSettings.AssetId == null)
            {
                var asset = context.Assets.Create(programSettings.AssetName, AssetCreationOptions.None);
                programOptions.AssetId = asset.Id;
                var accessPolicy = context.AccessPolicies.Create(
                    "ProgramLocator",
                    TimeSpan.FromDays(365),
                    AccessPermissions.List | AccessPermissions.Read);
                var locator = context.Locators.CreateLocator(LocatorType.OnDemandOrigin, asset, accessPolicy);
            }
            else
            {
                programOptions.AssetId = programSettings.AssetId;
            }
            var program = channel.Programs.Create(programOptions);
            return GetProgramDetails(program);
        }

        [HttpPut]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public void Update(string account, string id, [FromBody] ProgramUpdateSettings settings)
        {
            if(settings == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var program = MapProgram(account, id);
            if (program == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (program.State != ProgramState.Stopped)
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }
            program.Description = settings.Description ?? program.Description;
            program.ArchiveWindowLength = settings.ArchiveWindowLength == default(TimeSpan) ? program.ArchiveWindowLength : settings.ArchiveWindowLength;
            program.Update();
        }

        private IProgram MapProgram(string accountName, string programId)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(accountName);
            if (accountConfig == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var context = accountConfig.GetContext();
            string prgId = programId.GuidToProgramId();
            var program = context.Programs.Where(pr => pr.Id == prgId).FirstOrDefault();
            if (program == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return program;
        }

        private static MediaProgram GetProgramDetails(IProgram program)
        {
            var programDetails = EntityFactory.BuildProgramFromIProgram(program);
            return programDetails;
        }
    }
}
