using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Web.Api.Models;
using Microsoft.WindowsAzure.MediaServices.Client;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace MediaDashboard.Web.Api.Controllers
{
    public class OriginsController : ControllerBase
    {
        // GET: Origins
        [HttpGet]
        public List<MediaOrigin> GetOrigins(string account)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            return GetOrigins(accountConfig);
        }


        [HttpGet]
        public MediaOrigin GetOriginById(string account, string id)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            var context = accountConfig.GetContext();
            var endpoint = context.StreamingEndpoints
                .Where(se => se.Id == id.GuidToOriginId())
                .FirstOrDefault();
            if (endpoint == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            return EntityFactory.BuildOriginFromIStreamingEndpoint(endpoint);
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IOperation Start(string account, string id)
        {
            var origin = MapOrigin(account, id);
            if (origin.State != StreamingEndpointState.Stopped)
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }
            return origin.SendStartOperation();
        }

        [HttpPost]
        [Authorize(Roles = Role.OperatorOrHigher)]
        public IOperation Stop(string account, string id)
        {
            var origin = MapOrigin(account, id);
            if (origin.State != StreamingEndpointState.Running)
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }
            return origin.SendStopOperation();
        }

        [HttpPost]
        [Authorize(Roles = Role.Administrator)]
        public IOperation Create(string account, [FromBody] OriginSettings originSettings)
        {
            if (originSettings == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var mediaServices = App.Config.GetMediaServicesAccount(account);
            var context = mediaServices.GetContext();
            var originOptions = originSettings.GetCreationOptions();
            var operation = context.StreamingEndpoints.SendCreateOperation(originOptions);
            return operation;
        }


        [HttpDelete]
        [Authorize(Roles = Role.Administrator)]
        public IOperation Delete(string account, string id)
        {
            var origin = MapOrigin(account, id);
            if (origin.State != StreamingEndpointState.Stopped)
            {
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
            }
            var operation = origin.SendDeleteOperation();
            return operation;
        }

        [HttpPut]
        public IOperation Update(string account, string id, [FromBody] OriginUpdateSettings settings)
        {
            var origin = MapOrigin(account, id);
            if (settings.AllowList != null)
            {
                origin.AccessControl.IPAllowList = Models.IPRange.ToSdk(settings.AllowList);
            }

            origin.Description = settings.Description ?? origin.Description;
            return origin.SendUpdateOperation();

        }

        private IStreamingEndpoint MapOrigin(string accountName, string originId)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(accountName);
            if (accountConfig == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var context = accountConfig.GetContext();
            var endpoint = context.StreamingEndpoints
                .Where(se => se.Id == originId.GuidToOriginId())
                .FirstOrDefault();
            if (endpoint == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
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