using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class FailedRequestsMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        private Metric FailedRequestsMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.FailedRequestsMetricName,
                    DisplayName = MetricConstants.FailedRequestsMetricName,
                    Unit = MetricConstants.CountMetricUnit,
                    DisplayUnit = MetricConstants.CountMetricDisplayUnit,
                    AggregationType = MetricConstants.TotalMetricAggregationType
                };
            }
        }

        private Metric FailedRequestsBytesSentMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.FailedRequestsMetricName + MetricConstants.BytesSentMetricName,
                    DisplayName = MetricConstants.FailedRequestsMetricName + MetricConstants.BytesSentMetricName,
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
                    Name = MetricConstants.FailedRequestsMetricName + MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix,
                    DisplayName = MetricConstants.FailedRequestsMetricName + MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix,
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
                    Name = MetricConstants.FailedRequestsMetricName + MetricConstants.HttpStatusCodeE2ELatencyMetricNameSuffix,
                    DisplayName = MetricConstants.FailedRequestsMetricName + MetricConstants.HttpStatusCodeE2ELatencyMetricNameSuffix,
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

            var errorRequestCounts = telemetry.Where(t => 
                    Regex.IsMatch(t.MetricName, MetricConstants.FailedRequestsRatioMetricRegularExpression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
                    && !t.MetricName.Equals(MetricConstants.Http412MetricName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            decimal failedRequestCount = 0;
            decimal failedRequestBytesSent = 0;
            if (errorRequestCounts.Length > 0)
            {
                failedRequestCount = errorRequestCounts.Sum(t => t.Value);

            }

            var errorRequestBytes = telemetry.Where(t =>
                   Regex.IsMatch(t.MetricName, MetricConstants.FailedRequestsMetricRegularExpression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
                   && t.MetricName.EndsWith(MetricConstants.HttpStatusCodeBytesSentMetricNameSuffix, StringComparison.OrdinalIgnoreCase) 
                   && !t.MetricName.Equals(MetricConstants.Http412MetricName + MetricConstants.HttpStatusCodeBytesSentMetricNameSuffix, StringComparison.OrdinalIgnoreCase))
                   .ToArray();

            if (errorRequestBytes.Length > 0)
            {
                failedRequestBytesSent = errorRequestBytes.Sum(t => t.Value);
            }

            var newMetric = new Tuple<decimal, Metric>(failedRequestCount, FailedRequestsMetric);
            result.Add(newMetric);
            result.Add(new Tuple<decimal, Metric>(failedRequestBytesSent, FailedRequestsBytesSentMetric));

            // get the weighted server and e2e latency.
            var requestTimes = telemetry.Where(t =>
                   Regex.IsMatch(t.MetricName, MetricConstants.FailedRequestsMetricRegularExpression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
                   && t.MetricName.EndsWith(MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix, StringComparison.OrdinalIgnoreCase)
                   && ! t.MetricName.Equals(MetricConstants.Http412MetricName + MetricConstants.HttpStatusCodeServerLatencyMetricNameSuffix, StringComparison.OrdinalIgnoreCase));
            var serverLatency = errorRequestCounts.Zip(requestTimes, (m1, m2) => m1.Value * m2.Value).Sum();
            if (failedRequestCount > 0)
            {
                serverLatency = Math.Round(serverLatency / failedRequestCount, 3);
            }
            result.Add(new Tuple<decimal, Metric>(serverLatency, ServerLatencyMetric));

            requestTimes = telemetry.Where(t =>
                   Regex.IsMatch(t.MetricName, MetricConstants.FailedRequestsMetricRegularExpression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
                   && t.MetricName.EndsWith(MetricConstants.HttpStatusCodeE2ELatencyMetricNameSuffix, StringComparison.OrdinalIgnoreCase));
            var e2eLatency = errorRequestCounts.Zip(requestTimes, (m1, m2) => m1.Value * m2.Value).Sum();
            if (failedRequestCount > 0)
            {
                e2eLatency = Math.Round(e2eLatency / failedRequestCount, 3);
            }
            result.Add(new Tuple<decimal, Metric>(e2eLatency, E2ELatencyMetric));

            return result;
        }

    }
}
