using MediaDashboard.Common;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Common.Metrics;
using MediaDashboard.Operations.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Controllers
{
    public class OriginTelemetryController : ControllerBase
    {
        const int MetricsInterval = 30; // we get metrics aggregated over a 30 second interval.

        public IEnumerable<OriginMetricGroup> GetMetrics(string account, string id)
        {
            var originId = id.GuidToOriginId();
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var context = accountConfig.GetContext();
            var origin = context.StreamingEndpoints.Where(se => se.Id == originId).FirstOrDefault();
            if (origin == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var telemetryHelper = new TelemetryHelper(accountConfig, null, origin);
            var originMetrics = telemetryHelper.GetOriginTelemetry(origin.ScaleUnits ?? 0);

            return originMetrics.GroupBy(m => GetGroupName(m)).Select(group => CreateMetric(group.Key, group));
        }

        public OriginMetricGroup CreateMetric(string name, IEnumerable<EgressTelemetry> group)
        {
            var metrics = group.Select(m => new OriginMetric
            {
                Name = m.MetricName,
                Value = m.Value,
                Health = m.ComputeHealthState().Level
            }).ToArray();

            return new OriginMetricGroup
            {
                Name = name,
                Health = metrics.Select(m => m.Health).Max(),
                Metrics = metrics
            };
        }

        private static string GetGroupName(EgressTelemetry telemetry)
        {
            if (telemetry.MetricName.StartsWith("Http"))
                return telemetry.MetricName.Substring(4, 3);
            else if (telemetry.MetricName.StartsWith("Failed"))
                return MetricConstants.FailedRequessMetricDisplayName;
            else if (telemetry.MetricName.Equals(MetricConstants.BytesSentUtilizationRatioMetricName))
                return MetricConstants.BytesSentUtilizationRatioMetricDisplayName; // global metrics.
            else
                return MetricConstants.TotalRequestsMetricName;
        }
    }
}
