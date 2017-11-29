using System.Collections.Generic;
using System.Linq;
using MediaDashboard.Common;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Common.Metrics;
using MediaDashboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediaDashboard.Controllers
{
    public class ArchiveTelemetryController : ControllerBase
    {
        public IActionResult Get(string account, string id)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }

            var channelId = id.GuidToChannelId();
            var context = accountConfig.GetContext();
            var channel = context.Channels.Where(c => c.Id == channelId).FirstOrDefault();
            if (channel == null)
            {
                return NotFound();
            }

            var telemetryHelper = new TelemetryHelper(accountConfig, channel);
            var archiveMetrics = telemetryHelper.GetArchiveTelemetry();

            var telemetry = archiveMetrics.GroupBy(metric => metric.GroupId).Select(CreateMetric);
            return Ok(telemetry);
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
