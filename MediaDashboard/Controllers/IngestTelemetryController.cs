using System.Collections.Generic;
using System.Linq;
using MediaDashboard.Common;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Common.Metrics;
using Microsoft.AspNetCore.Mvc;
using MediaDashboard.Models;

namespace MediaDashboard.Controllers
{
    public class IngestTelemetryController : ControllerBase
    {

        [HttpGet("{id}")]
        public IActionResult Get(string account, string id)
        {
            var channelId = id.GuidToChannelId();
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }

            var context = accountConfig.GetContext();
            var channel = context.Channels.Where(ch => ch.Id == channelId).FirstOrDefault();
            if (channel == null)
            {
                return NotFound();
            }

            var telemetryHelper = new TelemetryHelper(accountConfig, channel);

            var metrics = telemetryHelper.GetIngestTelemetry().GroupBy(m => m.GroupId).Select(
                group => CreateMetric(group.Key, group.ToArray()));
            return Ok(metrics);
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
