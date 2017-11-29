using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaDashboard.Controllers
{
    public class ProgramsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllPrograms(string account)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }
            var programs = GetAllPrograms(accountConfig);
            return Ok(programs);
        }

        [HttpGet("{channelId}/Programs")]
        public IActionResult GetProgramsForChannel(string account, string channelId)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }
            var context = accountConfig.GetContext();
            string chId = channelId.GuidToChannelId();
            var channel = context.Channels.Where(ch => ch.Id == chId).FirstOrDefault();
            if (channel == null)
            {
                return NotFound();
            }

            var channels = channel.Programs.AsEnumerable().Select(GetProgramDetails);
            return Ok(channels);
        }

        [HttpGet("{id}/Urls")]
        public IActionResult GetUrls(string account, string id, [FromQuery] string originHostName = null)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }

            var context = accountConfig.GetContext();
            string prgId = id.GuidToProgramId();
            var program = context.Programs.Where(pr => pr.Id == prgId).FirstOrDefault();
            if (program == null)
            {
                return NotFound();
            }

            var urls = program.GetStreamingUrls();
            if (urls != null && originHostName != null)
            {
                urls.UpdateStreamingUrls(originHostName);
            }
            return Ok(urls);
        }

        public List<MediaProgram> GetAllPrograms(MediaServicesAccountConfig accountConfig)
        {
            var context = accountConfig.GetContext();
            var programs = context.Programs.ToList();
            var details = programs.Select(GetProgramDetails).ToList();
            return details;
        }

        public IActionResult Get(string account, string id)
        {
            var program = MapProgram(account, id);
            if (program == null) return NotFound();
            return Ok(GetProgramDetails(program));
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Start(string account, string id)
        {
            var program = MapProgram(account, id);
            if (program == null) return NotFound();
            if (program.State != ProgramState.Stopped)
            {
                return StatusCode(412);
            }
            var operation = program.SendStartOperation();
            Trace.TraceInformation("Starting program. Operation {0} on Program {1}", operation.Id, operation.TargetEntityId);
            return Ok(operation);
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Stop(string account, string id)
        {
            var program = MapProgram(account, id);
            if (program == null) return NotFound();
            if (program.State != ProgramState.Running)
            {
                return StatusCode(412);
            }
            var operation = program.SendStopOperation();
            Trace.TraceInformation("Stopping program. Operation {0} on Program {1}", operation.Id, operation.TargetEntityId);
            return Ok(operation);
        }

        [HttpDelete]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Delete(string account, string id)
        {
            var program = MapProgram(account, id);
            if (program == null) return NotFound();
            if (program.State != ProgramState.Stopped)
            {
                return StatusCode(412);
            }
            program.Delete();
            return Ok();
        }


        [HttpPost("Channels/{id}")]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Create(string account, string id, [FromBody] ProgramSettings programSettings)
        {
            if(programSettings == null || programSettings.Name == null)
            {
                return BadRequest();
            }

            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }

            var context = accountConfig.GetContext();
            var channelId = id.GuidToChannelId();
            var channel = context.Channels.Where(c => c.Id == channelId).FirstOrDefault();
            if(channel == null)
            {
                return NotFound();
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
            return Ok(GetProgramDetails(program));
        }

        [HttpPut]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Update(string account, string id, [FromBody] ProgramUpdateSettings settings)
        {
            if(settings == null)
            {
                return BadRequest();
            }

            var program = MapProgram(account, id);
            if (program == null)
            {
                return NotFound();
            }

            if (program.State != ProgramState.Stopped)
            {
                return StatusCode(412);
            }
            program.Description = settings.Description ?? program.Description;
            program.ArchiveWindowLength = settings.ArchiveWindowLength == default(TimeSpan) ? program.ArchiveWindowLength : settings.ArchiveWindowLength;
            program.Update();
            return Ok();
        }

        private IProgram MapProgram(string accountName, string programId)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(accountName);
            if (accountConfig == null)
            {
                return null;
            }

            var context = accountConfig.GetContext();
            string prgId = programId.GuidToProgramId();
            var program = context.Programs.Where(pr => pr.Id == prgId).FirstOrDefault();
            return program;
        }

        private static MediaProgram GetProgramDetails(IProgram program)
        {
            var programDetails = EntityFactory.BuildProgramFromIProgram(program);
            return programDetails;
        }
    }
}
