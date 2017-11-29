using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Models;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaDashboard.Controllers
{
    public class ChannelsController : ControllerBase
    {
        public IActionResult GetChannelById(string account, string id)
        {
            var mediaServices = App.Config.GetMediaServicesAccount(account);
            if (mediaServices == null)
            {
                return NotFound();
            }
            string chid = id.NimbusIdToRawGuid();
            var channel = mediaServices.GetContext().Channels
                .Where(ch => ch.Id == chid.GuidToChannelId())
                .FirstOrDefault();
            if (channel == null)
            {
                return NotFound();
            }

            var details = GetChannelDetails(mediaServices, channel);
            return Ok(details);
        }

        [HttpGet]
        public IActionResult GetAllChannels(string account)
        {
            var mediaServices = App.Config.GetMediaServicesAccount(account);
            if (mediaServices == null)
            {
                return NotFound();
            }

            return Ok(GetAllChannels(mediaServices));
        }

        [HttpPost]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Create(string account, [FromBody]ChannelSettings channelSettings)
        {
            if(channelSettings == null)
            {
                return BadRequest();
            }

            var mediaServices = App.Config.GetMediaServicesAccount(account);
            var context = mediaServices.GetContext();
            var channelOptions = channelSettings.GetChannelCreationOptions();
            var operation = context.Channels.SendCreateOperation(channelOptions);
            return Ok(operation);
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Start(string account, string id)
        {
            var channel = MapChannel(account, id);
            if (channel.State != ChannelState.Stopped)
            {
                return StatusCode(412);
            }
            return Ok(channel.SendStartOperation());
        }


        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Stop(string account, string id)
        {
            var channel = MapChannel(account, id);
            if (channel.State != ChannelState.Running)
            {
                return StatusCode(412);
            }
            return Ok(channel.SendStopOperation());
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Reset(string account, string id)
        {
            var channel = MapChannel(account, id);
            if (channel == null)
            {
                return BadRequest();
            }

            if (channel.State != ChannelState.Running)
            {
                return StatusCode(412);
            }
            return Ok(channel.SendResetOperation());
        }

        [HttpPut]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Update(string account, string id, [FromBody] ChannelUpdateSettings settings)
        {
            var channel = MapChannel(account, id);
            if (channel == null)
            {
              return BadRequest();
            }

            if (settings.IngestAllowList != null)
            {
                channel.Input.AccessControl = Models.IPRange.ToSdk(settings.IngestAllowList).CreateChannelAccessControl();
            }

            if (settings.PreviewAllowList != null)
            {
                channel.Preview.AccessControl  = Models.IPRange.ToSdk(settings.PreviewAllowList).CreateChannelAccessControl();
            }

            channel.Description = settings.Description ?? channel.Description;
            return Ok(channel.SendUpdateOperation());
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Slate(string account, string id, [FromBody] SlateSettings settings)
        {
            var channel = MapChannel(account, id);
            if (channel == null)
            {
                return BadRequest();
            }

            IOperation operation;
            if (settings.Duration > 0)
            {
                var duration = TimeSpan.FromSeconds(settings.Duration);
                operation = channel.SendShowSlateOperation(duration, settings.SlateAssetId);
            }
            else
            {
                operation = channel.SendHideSlateOperation();
            }
            return Ok(operation);
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult AdMarker(string account, string id, [FromBody] SlateSettings settings)
        {
            var channel = MapChannel(account, id);
            if (channel == null)
            {
              return BadRequest();
            }

            IOperation operation;
            if (settings.Duration > 0)
            {
                var duration = TimeSpan.FromSeconds(settings.Duration);
                operation = channel.SendStartAdvertisementOperation(duration, settings.CueId, settings.ShowSlate);
            }
            else
            {
                operation = channel.SendEndAdvertisementOperation(settings.CueId);
            }
            return Ok(operation);
        }


        [HttpDelete]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Delete(string account, string id)
        {
            var channel = MapChannel(account, id);
            if (channel == null)
            {
            return BadRequest();
            }

            if (channel.State == ChannelState.Running)
            {
                return StatusCode(412);
            }
            return Ok(channel.SendDeleteOperation());
        }

        internal List<MediaChannel> GetAllChannels(MediaServicesAccountConfig account)
        {
            var cloudContext = account.GetContext();
            var channels = cloudContext.Channels.ToList().OrderBy(ch => ch.Name).ToList();
            var channelDetails = channels.Select(channel => GetChannelDetails(account, channel)).ToList();
            GetMetrics(account, channelDetails);
            return channelDetails;
        }

        private IChannel MapChannel(string account, string id)
        {
            var mediaServices = App.Config.GetMediaServicesAccount(account);
            if (mediaServices == null)
            {
                return null;
            }

            var chid = id.NimbusIdToRawGuid();
            var channel = mediaServices.GetContext().Channels
                .Where(ch => ch.Id == chid.GuidToChannelId())
                .FirstOrDefault();
            if (channel == null)
            {
                return null;
            }

            return channel;
        }


        private MediaChannel GetChannelDetails(MediaServicesAccountConfig account, IChannel ch)
        {
            var channel = EntityFactory.BuildChannelFromIChannel(ch);
            return channel;
        }

        private void GetStatusLevel(MediaServicesAccountConfig account, MediaChannel channel)
        {
            var telemetryHelper = new TelemetryHelper(account, channel.Id, null);
            if (channel.EncodingType != ChannelEncodingType.None.ToString())
            {
                channel.EncodingHealth = GetEncodingLevel(telemetryHelper);
            }
            channel.IngestHealth = GetIngestLevel(telemetryHelper);
            channel.ArchiveHealth = GetArchiveLevel(telemetryHelper);
        }

        private HealthStatus GetEncodingLevel(TelemetryHelper telemetryHelper)
        {
            var aventusTelemetry = telemetryHelper.GetAventusTelemetry();
            return aventusTelemetry != null ?
                aventusTelemetry.ChannelHealth.HealthLevel.GetHealthStatus() :
                HealthStatus.Ignore;
        }

        public HealthStatus GetIngestLevel(TelemetryHelper telemetryHelper)
        {
            var metrics = telemetryHelper.GetIngestTelemetry();
            var ingestHealth = metrics.Select(metric => metric.ComputeHealthState().Level).DefaultIfEmpty(HealthStatus.Healthy).Max();
            return ingestHealth;
        }

        public HealthStatus GetArchiveLevel(TelemetryHelper telemetryHelper)
        {
            var metrics = telemetryHelper.GetArchiveTelemetry();
            var archiveHealth = metrics.Select(metric => metric.ComputeHealthState().Level).DefaultIfEmpty(HealthStatus.Healthy).Max();
            return archiveHealth;
        }

        private void GetProgramDetails(MediaServicesAccountConfig account, List<MediaChannel> channels)
        {
            var controller = new ProgramsController();
            var programs = controller.GetAllPrograms(account);
            channels.ForEach(channel =>
            {
                channel.Programs = programs
                    .Where(program => program.ChannelId == channel.Id)
                    .OrderBy(program => program.State)
                    .ToList();
            });
        }

        private void GetOriginDetails(MediaServicesAccountConfig account, List<MediaChannel> channels)
        {
            List<MediaOrigin> origins = GetAccountOrigins(account);
            channels.ForEach(channel => GetOriginDetails(account, channel));
        }

        private static List<MediaOrigin> GetAccountOrigins(MediaServicesAccountConfig account)
        {
            var controller = new OriginsController();
            var origins = controller.GetOrigins(account);
            return origins;
        }

        private void GetOriginDetails(MediaServicesAccountConfig account, MediaChannel channel)
        {
            List<MediaOrigin> origins = GetAccountOrigins(account);
            var origin = origins.FindOrigin(channel.NameShort);
            if (origin != null)
            {
                channel.OriginId = origin.Id.GuidToOriginId();
                channel.OriginHostName = origin.HostName;
            }
        }

        private void GetMetrics(MediaServicesAccountConfig account, List<MediaChannel> channels)
        {
            channels.ForEach(channel => channel.EncodingHealth = channel.IngestHealth = channel.ArchiveHealth = channel.OriginHealth = HealthStatus.Ignore);
            var runningChanels = channels.Where(ch => ch.State == ChannelState.Running.ToString()).ToList();
            Parallel.ForEach(runningChanels, (channel) => GetStatusLevel(account, channel));
        }
    }
}
