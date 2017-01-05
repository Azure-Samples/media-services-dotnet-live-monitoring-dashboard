using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Metrics;
using MediaDashboard.Common.Metrics.MediaServices;
using MediaDashboard.Common.TelemetryStorageClient;
using Microsoft.WindowsAzure.MediaServices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MediaDashboard.Common.Helpers
{
    public class TelemetryHelper
    {

        private string _channelId;

        private string _originId;

        private ChannelMetrics _channelMetrics;

        private StreamingEndpointMetrics _originMetrics;

        public TelemetryHelper(MediaServicesAccountConfig account, IChannel channel, IStreamingEndpoint origin = null):
            this(account, channel?.Id.NimbusIdToRawGuid(), origin?.Id.NimbusIdToRawGuid())
        {
        }

        public TelemetryHelper(MediaServicesAccountConfig account, string channelId, string originId)
        {
            if (account.TelemetryStorage != null)
            {
                var telemetryStorage = new TelemetryStorage(account.TelemetryStorage);

                if (channelId != null)
                {
                    _channelId = channelId;
                    _channelMetrics = telemetryStorage.GetRecentChannelMetrics(
                        new Guid(account.Id),
                       new Guid(channelId),
                        20);
                }

                if (originId != null)
                {
                    _originId = originId;
                    _originMetrics = telemetryStorage.GetRecentStreamingEndpointMetrics(
                        new Guid(account.Id),
                       new Guid(originId.NimbusIdToRawGuid()),
                        20);
                }
            }
        }

        public AventusTelemetry GetAventusTelemetry()
        {
            var aventusTelemetry = _channelMetrics.AventusTelemetry.FirstOrDefault();
            if (aventusTelemetry != null && aventusTelemetry.Data.StartsWith("{"))
            {
                try
                {
                    return JsonConvert.DeserializeObject<AventusTelemetry>(aventusTelemetry.Data);
                }
                catch (JsonException je)
                {
                    Trace.TraceError("Failed to parse aventus telemery for channel:{0} with error:{1}\n Value:", _channelId, je, aventusTelemetry.Data);
                }
            }

            return null;
        }

        public IEnumerable<ChannelHeartbeat> GetIngestMetrics()
        {
            // Group by bitrate and pick latest for each track.
            return _channelMetrics?.ChannelHeartbeats
                .GroupBy(c => c.Bitrate)
                .Select(g => g.First())
                .OrderBy(c => c.Bitrate);
        }

        public IEnumerable<ChannelFragmentDiscarded> GetArchiveMetrics()
        {
            return _channelMetrics?.ChannelFragmentsDiscarded
                .GroupBy(m => m.Bitrate)
                .Select(g => g.First());
        }

        int[] statusCodes = { 200, 304, 400, 403, 404, 410, 412, 415, 500, 503 };

        public IEnumerable<StreamingEndpointRequestLog> GetOriginMetrics()
        {
            Validate.NotNull(_originMetrics, "Origin id is not given");
            if (!_originMetrics.StreamingEndpointRequestLogs.Any())
            {
                return Enumerable.Empty<StreamingEndpointRequestLog>();
            }

            var maxTime = _originMetrics.StreamingEndpointRequestLogs.Max(s => s.ObservedTime) - TimeSpan.FromMinutes(1);
            
            var originMetrics = _originMetrics.StreamingEndpointRequestLogs
                .Where(s => s.ObservedTime >= maxTime - TimeSpan.FromMinutes(1))
                .GroupBy(log => log.StatusCode)
                .Select(group => group.OrderByDescending(log => log.ObservedTime).First())
                .ToArray();

            var template = originMetrics.FirstOrDefault();
            var metrics = statusCodes.Select(statusCode =>
            {
                var originMetric = originMetrics.FirstOrDefault(metric => metric.StatusCode == statusCode);
                return originMetric ?? new StreamingEndpointRequestLog(template, statusCode);
            }).ToList();

            return metrics;
        }

        public List<EgressTelemetry> GetOriginTelemetry(int reservedUnits)
        {
            var metricCalculator = new MetricCalculator();
            var originalMetrics = GetOriginMetrics().ToList().ToEgressTelemetry(_originId, reservedUnits);
            var computedMetrics = metricCalculator.GenerateEgressMetrics(originalMetrics);
            var allMetrics = originalMetrics.Union(computedMetrics.Select(m => m.Item1)).ToList();
            foreach(var metric in allMetrics)
            {
                Metric metricDefinition;
                App.Config.Parameters.Metrics.TryGetValue(metric.MetricName, out metricDefinition);
                metric.Metric = metricDefinition;
            }
            return allMetrics;
        }

        public List<IngestTelemetry> GetIngestTelemetry()
        {
            var metricCalculator = new MetricCalculator();
            var originalMetrics = GetIngestMetrics().ToList().ToIngestTelemetry(_channelId, TimeSpan.FromSeconds(2));
            var computedMetrics = metricCalculator.GenerateIngestMetrics(originalMetrics);
            var allMetrics = originalMetrics.Union(computedMetrics.Select(m => m.Item1)).ToList();
            foreach (var metric in allMetrics)
            {
                Metric metricDefinition;
                App.Config.Parameters.Metrics.TryGetValue(metric.MetricName, out metricDefinition);
                metric.Metric = metricDefinition;
            }
            return allMetrics;
        }

        public List<ProgramTelemetry> GetArchiveTelemetry()
        {
            var metricCalculator = new MetricCalculator();
            var originalMetrics = GetArchiveMetrics().ToList().ToArchiveTelemetry(GetIngestMetrics().ToList());
            var computedMetrics = metricCalculator.GenerateProgramMetrics(originalMetrics);
            var allMetrics = originalMetrics.Union(computedMetrics.Select(m => m.Item1)).ToList();
            foreach (var metric in allMetrics)
            {
                Metric metricDefinition;
                App.Config.Parameters.Metrics.TryGetValue(metric.MetricName, out metricDefinition);
                metric.Metric = metricDefinition;
            }
            return allMetrics;

        }
    }
}
