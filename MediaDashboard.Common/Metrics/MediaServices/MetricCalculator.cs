using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{

    public class MetricCalculator : IMetricCalculator
    {
        internal static readonly IEnumerable<IMetricCalculatorStrategy> TotalStrategies = new IMetricCalculatorStrategy[]
        {
            new FailedRequestsMetricCalculatorStrategy(),
            new TotalRequestsMetricCalculatorStrategy()
        };

        internal static readonly IEnumerable<IMetricCalculatorStrategy> RateStrategies = new IMetricCalculatorStrategy[] 
        {
            new FragmentsDroppedRateMetricCalculatorStrategy(),
            new HttpStatusCodeRateMetricCalculatorStrategy(),
            new HttpStatusCodeBytesSentRateMetricCalculatorStrategy(),
            new RequestsRateMetricCalculatorStrategy(),
            new BytesSentRateMetricCalculatorStrategy(),
        };

        internal static readonly IEnumerable<IMetricCalculatorStrategy> RatioStrategies = new IMetricCalculatorStrategy[] 
        {
            new BitrateRatioMetricCalculatorStrategy(),
            new HttpStatusCodeRatioMetricCalculatorStrategy(),
            new FailedRequestsRatioMetricCalculatorStrategy(),
            new RequestsRatioMetricCalculatorStrategy(),
            new BytesSentUtilizationRatio()
        };

        internal static readonly IEnumerable<IMetricCalculatorStrategy> AllStrategies = TotalStrategies.Concat(RateStrategies).Concat(RatioStrategies);

        private readonly List<IMetricCalculatorStrategy> strategies;

        public MetricCalculator()
            : this(AllStrategies)
        {
        }

        public MetricCalculator(IEnumerable<IMetricCalculatorStrategy> strategies)
        {
            this.strategies = strategies.ToList();
        }

        public IReadOnlyCollection<IMetricCalculatorStrategy> Strategies
        {
            get { return this.strategies.AsReadOnly(); }
        }

        public TupleList<IngestTelemetry, Metric> GenerateIngestMetrics(List<IngestTelemetry> ingestTelemetryTuples)
        {
            return GenerateMetrics(
                MetricType.Ingest,
                ingestTelemetryTuples,
                (groupId, timestamp, newMetric, newMetricValue) =>
                {
                    var keys = groupId.Split(new[] { MetricConstants.MetricGroupIdSeparator }, StringSplitOptions.None);
                    return new IngestTelemetry
                    {
                        ChannelId = keys[0],
                        StreamId = keys[1],
                        TrackId = keys[2],
                        Bitrate = keys[3],
                        MetricName = newMetric.Name,
                        Timestamp = timestamp,
                        Value = newMetricValue
                    };
                });
        }

        public TupleList<ProgramTelemetry, Metric> GenerateProgramMetrics(List<ProgramTelemetry> programTelemetryTuples)
        {
            return GenerateMetrics(
                MetricType.Archive,
                programTelemetryTuples,
                (groupId, timestamp, newMetric, newMetricValue) =>
                {
                    var keys = groupId.Split(new[] { MetricConstants.MetricGroupIdSeparator }, StringSplitOptions.None);
                    return new ProgramTelemetry
                    {
                        ProgramId = keys[0],
                        StreamId = keys[1],
                        TrackId = keys[2],
                        Bitrate = keys[3],
                        MetricName = newMetric.Name,
                        Timestamp = timestamp,
                        Value = newMetricValue
                    };
                });
        }

        public TupleList<EgressTelemetry, Metric> GenerateEgressMetrics(List<EgressTelemetry> egressTelemetry)
        {
            var miscellaneous = egressTelemetry.FirstOrDefault()?.Miscellaneous;
            return GenerateMetrics(
                MetricType.Origin,
                egressTelemetry,
                (groupId, timestamp, newMetric, newMetricValue) =>
                {
                    return new EgressTelemetry
                    {
                        OriginId = groupId,
                        MetricName = newMetric.Name,
                        Timestamp = timestamp,
                        Value = newMetricValue,
                        Miscellaneous = miscellaneous
                    };
                });
        }

        private TupleList<TCurrent, Metric> GenerateMetrics<TCurrent>(
            MetricType type,
            List<TCurrent> telemetry,
            Func<string, DateTime, Metric, decimal, TCurrent> createNewMetric)
            where TCurrent : ITelemetry
        {
            var calculatedMetrics = new TupleList<TCurrent, Metric>();
            var filteredStrategies = strategies.Where(s => s.Type == type).ToArray();

            var telemetryGroups = telemetry.GroupBy(t => t.GroupId);
            foreach (var telemetryGroup in telemetryGroups)
            {
                var allMetrics = telemetryGroup.ToList();
                var timestamp = allMetrics.First().Timestamp;
                foreach (var strategy in filteredStrategies)
                {
                    var newMetricValueTuples = strategy.CalculateMetrics(allMetrics);
                    var newMetrics = newMetricValueTuples.Select(
                        nmvt => new Tuple<TCurrent, Metric>(
                            createNewMetric(telemetryGroup.Key, timestamp, nmvt.Item2, nmvt.Item1),
                            nmvt.Item2));

                    allMetrics.AddRange(newMetrics.Select(nmvt => nmvt.Item1));
                    calculatedMetrics.AddRange(newMetrics);
                }
            }

            return calculatedMetrics;
        }
    }
}
