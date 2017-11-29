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
    public class OriginsController : ControllerBase
    {
        // GET: Origins
        [HttpGet]
        public IActionResult GetOrigins(string account)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }

            var origins = GetOrigins(accountConfig);
            return Ok(origins);
        }


        [HttpGet]
        public IActionResult GetOriginById(string account, string id)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }

            var context = accountConfig.GetContext();
            var endpoint = context.StreamingEndpoints
                .Where(se => se.Id == id.GuidToOriginId())
                .FirstOrDefault();
            if (endpoint == null)
            {
                return NotFound();
            }

            var origin = EntityFactory.BuildOriginFromIStreamingEndpoint(endpoint);
            return Ok(origin); 
    }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Start(string account, string id)
        {
            var origin = MapOrigin(account, id);
            if (origin.State != StreamingEndpointState.Stopped)
            {
                return StatusCode(412);
            }

            return Ok(origin.SendStartOperation());
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IActionResult Stop(string account, string id)
        {
            var origin = MapOrigin(account, id);
            if (origin.State != StreamingEndpointState.Running)
            {
                return StatusCode(412);
            }

            return Ok(origin.SendStartOperation());
    }

        [HttpPost]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Create(string account, [FromBody] OriginSettings originSettings)
        {
            if (originSettings == null)
            {
                return BadRequest();
            }

            var mediaServices = App.Config.GetMediaServicesAccount(account);
            var context = mediaServices.GetContext();
            var originOptions = originSettings.GetCreationOptions();
            var operation = context.StreamingEndpoints.SendCreateOperation(originOptions);
            return Ok(operation);
        }


        [HttpDelete]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult Delete(string account, string id)
        {
            var origin = MapOrigin(account, id);
            if (origin == null)
            {
                return NotFound();
            }

            if (origin.State != StreamingEndpointState.Stopped)
            {
                return StatusCode(412);
            }
            var operation = origin.SendDeleteOperation();
            return Ok(operation);
        }

        [HttpPut]
        public IActionResult Update(string account, string id, [FromBody] OriginUpdateSettings settings)
        {
            var origin = MapOrigin(account, id);
            if (origin == null)
            {
                return NotFound();
            }

            if (settings.AllowList != null)
            {
                origin.AccessControl.IPAllowList = Models.IPRange.ToSdk(settings.AllowList);
            }

            origin.Description = settings.Description ?? origin.Description;
            return Ok(origin.SendUpdateOperation());

        }

        private IStreamingEndpoint MapOrigin(string accountName, string originId)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(accountName);
            if (accountConfig == null)
            {
                return null;
            }
            var context = accountConfig.GetContext();
            var endpoint = context.StreamingEndpoints
                .Where(se => se.Id == originId.GuidToOriginId())
                .FirstOrDefault();
            if (endpoint == null)
            {
                return null;
            }
            return endpoint;
        }

        internal List<MediaOrigin> GetOrigins(MediaServicesAccountConfig account)
        {
            var context = account.GetContext();
            var origins = context.StreamingEndpoints.ToList()
                .OrderBy(e => e.Name)
                .Select(EntityFactory.BuildOriginFromIStreamingEndpoint)
                .ToList();
            GetMetrics(account, origins);
            return origins;
        }

        private void GetMetrics(MediaServicesAccountConfig account, List<MediaOrigin> origins)
        {
            origins.ForEach(origin => origin.Health = HealthStatus.Ignore);
            var runningOrigins = origins.Where(o => o.State == StreamingEndpointState.Running.ToString());
            Parallel.ForEach(runningOrigins, origin =>
            {
                var telemetryHelper = new TelemetryHelper(account, null, origin.Id);

                var metrics = telemetryHelper.GetOriginTelemetry(origin.ReservedUnits);
                origin.Health = metrics.Select(metric => metric.ComputeHealthState().Level).DefaultIfEmpty(HealthStatus.Healthy).Max();
            });
        }
    }
}
