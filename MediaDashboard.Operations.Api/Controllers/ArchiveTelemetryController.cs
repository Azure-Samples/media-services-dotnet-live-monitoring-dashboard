using MediaDashboard.Common;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Common.Metrics;
using MediaDashboard.Web.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MediaDashboard.Web.Api.Controllers
{
    public class ArchiveTelemetryController : ControllerBase
    {

        public IEnumerable<ArchiveMetricGroup> Get(string account, string id)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var channelId = id.GuidToChannelId();
            var context = accountConfig.GetContext();
            var channel = context.Channels.Where(c => c.Id == channelId).FirstOrDefault();
            if (channel == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var telemetryHelper = new TelemetryHelper(accountConfig, channel);
            var archiveMetrics = telemetryHelper.GetArchiveTelemetry();

            return archiveMetrics.GroupBy(metric => metric.GroupId).Select(CreateMetric);

        }

        private static ArchiveMetricGroup CreateMetric(IEnumerable<ProgramTelemetry> archiveMetrics)
        {
            var firstMetric = archiveMetrics.First();
            var metrics = archiveMetrics.Select(metric =>
            {
                return new ArchiveMetric
                {
                    Name = metric.MetricName,
                    Value = metric.Value,
                    Health = metric.ComputeHealthState().Level
                };
            }).ToArray();

            return new ArchiveMetricGroup
            {
                TrackType = firstMetric.StreamId,
                TrackName = firstMetric.TrackId,
                Bitrate = firstMetric.Bitrate,
                Health = metrics.Select(m => m.Health).Max(),
                Metrics = metrics
            };
        }
    }
}