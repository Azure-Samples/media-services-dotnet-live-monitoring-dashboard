using MediaDashboard.Common;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Common.Metrics;
using MediaDashboard.Web.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MediaDashboard.Web.Api.Controllers
{
    public class IngestTelemetryController : ControllerBase
    {

        public IEnumerable<IngestMetricGroup> Get(string account, string id)
        {
            var channelId = id.GuidToChannelId();
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var context = accountConfig.GetContext();
            var channel = context.Channels.Where(ch => ch.Id == channelId).FirstOrDefault();
            if (channel == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var telemetryHelper = new TelemetryHelper(accountConfig, channel);

            return telemetryHelper.GetIngestTelemetry().GroupBy(m => m.GroupId).Select(
                group => CreateMetric(group.Key, group.ToArray()));
        }

        private static IngestMetricGroup CreateMetric(string name, IngestTelemetry[] metrics)
        {
            var ingestMetrics = metrics.Select(m => new IngestMetric
            {
                Name = m.MetricName,
                Value = m.Value,
                Health = m.ComputeHealthState().Level
            }).ToArray();

            return new IngestMetricGroup
            {
                TrackType = metrics[0].StreamId,
                TrackName = metrics[0].TrackId,
                Metrics = ingestMetrics,
                Health = ingestMetrics.Max( m => m.Health)
            };
        }

    }
}