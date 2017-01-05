using MediaDashboard.Common.Data;
using MediaDashboard.Common.TelemetryStorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{

    public static class MetricsMapperExtensions
    {

        internal static readonly Metric TotalFragmentsDroppedMetric = new Metric
        {
            Name = MetricConstants.TotalFragmentsDroppedMetricName,
            DisplayName = MetricConstants.TotalFragmentsDroppedMetricDisplayName,
            Unit = MetricConstants.CountMetricUnit,
            DisplayUnit = MetricConstants.CountMetricDisplayUnit,
            AggregationType = MetricConstants.TotalMetricAggregationType
        };

        public static List<EgressTelemetry> ToEgressTelemetry(this List<StreamingEndpointRequestLog> egressMetrics, string originId, int originReservedUnits)
        {
            var result = new List<EgressTelemetry>();

            foreach (var egressMetric in egressMetrics)
            {

                var metricName = string.Format("{0}{1}", MetricConstants.HttpStatusCodeMetricNamePrefix, egressMetric.StatusCode);
                var telemetry = new EgressTelemetry
                {
                    OriginId = egressMetric.EntityId.ToString(),
                    Timestamp = egressMetric.ObservedTime,
                    MetricName = metricName,
                    Value = egressMetric.RequestCount,
                    Miscellaneous = originReservedUnits
                };
                result.Add(telemetry);

                metricName = string.Format("{0}{1}{2}", MetricConstants.HttpStatusCodeMetricNamePrefix, egressMetric.StatusCode, MetricConstants.HttpStatusCodeBytesSentMetricNameSuffix);
                telemetry = new EgressTelemetry
                {
                    OriginId = egressMetric.EntityId.ToString(),
                    Timestamp = egressMetric.ObservedTime,
                    MetricName = metricName,
                    Value = Math.Round(egressMetric.BytesSent  * 8m / (1024 * 1024), 3),
                    Miscellaneous = originReservedUnits
                };
                result.Add(telemetry);

                metricName = string.Format("{0}{1}{2}", MetricConstants.HttpStatusCodeMetricNamePrefix, egressMetric.StatusCode, MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix);
                telemetry = new EgressTelemetry
                {
                    OriginId = egressMetric.EntityId.ToString(),
                    Timestamp = egressMetric.ObservedTime,
                    MetricName = metricName,
                    Value = egressMetric.ServerLatency,
                    Miscellaneous = originReservedUnits
                };
                result.Add(telemetry);

                metricName = string.Format("{0}{1}{2}", MetricConstants.HttpStatusCodeMetricNamePrefix, egressMetric.StatusCode, MetricConstants.HttpStatusCodeE2ELatencyMetricNameSuffix);
                telemetry = new EgressTelemetry
                {
                    OriginId = egressMetric.EntityId.ToString(),
                    Timestamp = egressMetric.ObservedTime,
                    MetricName = metricName,
                    Value = egressMetric.ServerLatency,
                    Miscellaneous = originReservedUnits
                };
                result.Add(telemetry);
            }

            return result;
        }

        public static List<IngestTelemetry> ToIngestTelemetry(this List<ChannelHeartbeat> ingestMetrics, string channelId, TimeSpan? fragmentDuration)
        {
            var result = new List<IngestTelemetry>();

            foreach (var ingestMetric in ingestMetrics)
            {
                var telemetry = new IngestTelemetry
                {
                    ChannelId = ingestMetric.EntityId.ToString(),
                    TrackId = ingestMetric.TrackName,
                    StreamId = ingestMetric.TrackType,
                    Timestamp = ingestMetric.ObservedTime,
                    Bitrate = ingestMetric.Bitrate.ToString(),
                    MetricName = MetricConstants.EncodedBitrateMetricName,
                    Value = ingestMetric.Bitrate,
                    Miscellaneous = fragmentDuration
                };
                result.Add(telemetry);

                telemetry = new IngestTelemetry
                {
                    ChannelId = ingestMetric.EntityId.ToString(),
                    TrackId = ingestMetric.TrackName,
                    StreamId = ingestMetric.TrackType,
                    Bitrate = ingestMetric.Bitrate.ToString(),
                    Timestamp = ingestMetric.ObservedTime,
                    MetricName = MetricConstants.IncomingBitrateMetricName,
                    Value = ingestMetric.IncomingBitrate,
                    Miscellaneous = fragmentDuration
                };
                result.Add(telemetry);

                telemetry = new IngestTelemetry
                {
                    ChannelId = ingestMetric.EntityId.ToString(),
                    TrackId = ingestMetric.TrackName,
                    StreamId = ingestMetric.TrackType,
                    Bitrate = ingestMetric.Bitrate.ToString(),
                    Timestamp = ingestMetric.ObservedTime,
                    MetricName = MetricConstants.LastTimestampMetricName,
                    Value = ingestMetric.LastTimestamp,
                    Miscellaneous = fragmentDuration
                };
                result.Add(telemetry);

                telemetry = new IngestTelemetry
                {
                    ChannelId = ingestMetric.EntityId.ToString(),
                    TrackId = ingestMetric.TrackName,
                    StreamId = ingestMetric.TrackType,
                    Bitrate = ingestMetric.Bitrate.ToString(),
                    Timestamp = ingestMetric.ObservedTime,
                    MetricName = MetricConstants.DiscontinuityCountMetricName,
                    Value = ingestMetric.DiscontinuityCount,
                    Miscellaneous = fragmentDuration
                };
                result.Add(telemetry);

                telemetry = new IngestTelemetry
                {
                    ChannelId = ingestMetric.EntityId.ToString(),
                    TrackId = ingestMetric.TrackName,
                    StreamId = ingestMetric.TrackType,
                    Bitrate = ingestMetric.Bitrate.ToString(),
                    Timestamp = ingestMetric.ObservedTime,
                    MetricName = MetricConstants.OverlapCountMetricName,
                    Value = ingestMetric.OverlapCount,
                    Miscellaneous = fragmentDuration
                };
                result.Add(telemetry);
            }

            return result;
        }

        public static List<ProgramTelemetry> ToArchiveTelemetry(this List<ChannelFragmentDiscarded> archiveMetrics, List<ChannelHeartbeat> ingestMetrics)
        {
            var result = new List<ProgramTelemetry>();
            foreach(var ingestMetric in ingestMetrics)
            {
                var telemetry = new ProgramTelemetry
                {
                    ProgramId = ingestMetric.EntityId.ToString(),
                    StreamId = ingestMetric.TrackType,
                    TrackId = ingestMetric.TrackName,
                    Bitrate = ingestMetric.Bitrate.ToString(),
                    Timestamp = ingestMetric.ObservedTime,
                    MetricName = MetricConstants.TotalFragmentsDroppedMetricName,
                    Value = 0m
                };
                var archiveMetric = archiveMetrics
                    .FirstOrDefault(m => m.TrackName == ingestMetric.TrackName && m.Bitrate == ingestMetric.Bitrate);
                if (archiveMetric != null)
                {
                    telemetry.Value = archiveMetric.Count;
                    telemetry.VirtualPath = archiveMetric.VirtualPath;
                };
                result.Add(telemetry);
            }

            return result;
        }

        public static Metric ToMetric(this Metric metric)
        {
            return new Metric
                {
                    Name = metric.Name,
                    DisplayName = metric.DisplayName,
                    Unit = metric.Unit,
                    DisplayUnit = metric.GetDisplayUnit(),
                    AggregationType = metric.AggregationType
                };
        }

        private static string GetDisplayUnit(this Metric metric)
        {
            switch (metric.Unit)
            {
                case MetricConstants.CountMetricUnit:
                    return MetricConstants.CountMetricDisplayUnit;
                case MetricConstants.CountPerSecondMetricUnit:
                    return MetricConstants.CountPerSecondMetricDisplayUnit;
                case MetricConstants.CountPerMinuteMetricUnit:
                    return MetricConstants.CountPerMinuteMetricDisplayUnit;
                case MetricConstants.MillisecondsMetricUnit:
                    return MetricConstants.MillisecondsMetricDisplayUnit;
                case MetricConstants.MegabitsMetricUnit:
                    return MetricConstants.MegabitsMetricDisplayUnit;
                case MetricConstants.MegabitsPerSecondMetricUnit:
                    return MetricConstants.MegabitsPerSecondMetricDisplayUnit;
                default:
                    return metric.Unit;
            }
        }

        private static void UpdateMetricUnit(ITelemetry metric)
        {
            if (metric.Metric.Unit.Equals(MetricConstants.BitsPerSecondMetricUnit))
            {
                metric.Value /= 1000 * 1000;
                metric.Metric.Unit = MetricConstants.MegabitsPerSecondMetricUnit;
            }
            else if (metric.Metric.Unit.Equals(MetricConstants.BytesMetricUnit))
            {
                metric.Value *= 8;
                metric.Value /= 1000 * 1000;
                metric.Metric.Unit = MetricConstants.MegabitsMetricUnit;
            }
        }

        public static string GetEncodingMetricName(string ruleID)
        {
            string metricName = string.Empty;
            switch (ruleID)
            {
                case "3fcd59ad-673b-4421-bd5d-c3d826ffa8f9":
                    metricName = "Egress/Packaging Rate Mismatch";
                    break;
                case "6d33d613-ca96-4198-bc88-9b2b3f446474":
                    metricName = "Incoming Signal Received Time";
                    break;
                case "f24be797-09ac-45e5-ab1a-f26fdb493546":
                    metricName = "Successful Publish Time Exceeded";
                    break;
                case "c7bf38aa-bd42-11e4-8dfc-aa07a5b093db":
                    metricName = "Incoming Expected Stream Data Loss Exceeded";
                    break;
                case "8113fab1-f917-4c3f-855e-cf0aa947e141":
                    metricName = "Valid Video Decoding Time Exceeded";
                    break;
                case "04fd3a59-11a9-4b99-96f6-c71dc78c9eb9":
                    metricName = "Decoded Video Frame Rate Mismatch";
                    break;
            }
            return metricName;
        }
    }
}
