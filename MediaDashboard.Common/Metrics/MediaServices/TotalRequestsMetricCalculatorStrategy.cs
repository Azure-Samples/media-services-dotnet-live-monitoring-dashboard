using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class TotalRequestsMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        private Metric TotalRequestsMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.TotalRequestsMetricName,
                    DisplayName = MetricConstants.TotalRequestsMetricName,
                    Unit = MetricConstants.CountMetricUnit,
                    DisplayUnit = MetricConstants.CountMetricDisplayUnit,
                    AggregationType = MetricConstants.TotalMetricAggregationType
                };
            }
        }

        private Metric BytesSentMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.BytesSentMetricName,
                    DisplayName = MetricConstants.BytesSentMetricName,
                    Unit = MetricConstants.MegabitsMetricUnit,
                    DisplayUnit = MetricConstants.MegabitsMetricDisplayUnit,
                    AggregationType = MetricConstants.TotalMetricAggregationType
                };
            }
        }

        public Metric ServerLatencyMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.TotalRequestsMetricName + MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix,
                    DisplayName = MetricConstants.TotalRequestsMetricName + MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix,
                    Unit = MetricConstants.MillisecondsMetricUnit,
                    DisplayUnit = MetricConstants.MillisecondsMetricDisplayUnit,
                    AggregationType = MetricConstants.AverageMetricAggregationType
                };
            }
        }

        public Metric E2ELatencyMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.TotalRequestsMetricName + MetricConstants.HttpStatusCodeE2ELatencyMetricNameSuffix,
                    DisplayName = MetricConstants.TotalRequestsMetricName + MetricConstants.HttpStatusCodeE2ELatencyMetricNameSuffix,
                    Unit = MetricConstants.MillisecondsMetricUnit,
                    DisplayUnit = MetricConstants.MillisecondsMetricDisplayUnit,
                    AggregationType = MetricConstants.AverageMetricAggregationType
                };
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetry)
            where TCurrent : ITelemetry
        {
            var result = new TupleList<decimal, Metric>();


            // Look at all HttpXXX metrics.
            var requestCounts = telemetry.Where(t =>
                   t.MetricName.StartsWith(MetricConstants.HttpStatusCodeMetricNamePrefix, StringComparison.OrdinalIgnoreCase)
                   && t.MetricName.Length == 7);

            var totalRequests = requestCounts.Sum(metric => metric.Value);

            result.Add(new Tuple<decimal, Metric>(totalRequests, TotalRequestsMetric));

            var bytesSent = telemetry.Where(t =>
                   t.MetricName.StartsWith(MetricConstants.HttpStatusCodeMetricNamePrefix, StringComparison.OrdinalIgnoreCase)
                   && t.MetricName.EndsWith(MetricConstants.HttpStatusCodeBytesSentMetricNameSuffix, StringComparison.OrdinalIgnoreCase))
                 .Sum(metric => metric.Value);

            result.Add(new Tuple<decimal, Metric>(bytesSent, BytesSentMetric));

            // get the weighted server and e2e latency.
            var requestTimes = telemetry.Where(t =>
                   t.MetricName.StartsWith(MetricConstants.HttpStatusCodeMetricNamePrefix, StringComparison.OrdinalIgnoreCase)
                   && t.MetricName.EndsWith(MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix, StringComparison.OrdinalIgnoreCase));
            var serverLatency = requestCounts.Zip(requestTimes, (m1, m2) => m1.Value * m2.Value).Sum();
            serverLatency = (totalRequests > 0) ? Math.Round(serverLatency / totalRequests, 3) : 0;
            result.Add(new Tuple<decimal, Metric>(serverLatency, ServerLatencyMetric));

            requestTimes = telemetry.Where(t =>
                   t.MetricName.StartsWith(MetricConstants.HttpStatusCodeMetricNamePrefix, StringComparison.OrdinalIgnoreCase)
                   && t.MetricName.EndsWith(MetricConstants.HttpStatusCodeE2ELatencyMetricNameSuffix, StringComparison.OrdinalIgnoreCase));
            var e2eLatency = requestCounts.Zip(requestTimes, (m1, m2) => m1.Value * m2.Value).Sum();
            e2eLatency = (totalRequests > 0) ? Math.Round(e2eLatency / totalRequests, 3) : 0;
            result.Add(new Tuple<decimal, Metric>(e2eLatency, E2ELatencyMetric));

            return result;
        }

    }
}
