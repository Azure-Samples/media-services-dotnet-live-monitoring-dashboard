using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class HttpStatusCodeBytesSentRateMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;

        public HttpStatusCodeBytesSentRateMetricCalculatorStrategy()
        {
            monitoringIntervalInSeconds = MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            return telemetryTuples.Where( t => 
                t.MetricName.StartsWith(MetricConstants.HttpStatusCodeMetricNamePrefix, StringComparison.OrdinalIgnoreCase)
                && t.MetricName.EndsWith(MetricConstants.HttpStatusCodeBytesSentMetricNameSuffix, StringComparison.OrdinalIgnoreCase))
                    .Select(t =>
                    {
                        var newMetric = new Tuple<decimal, Metric>(
                            Math.Round(t.Value / monitoringIntervalInSeconds, 3),
                            GetHttpStatusCodeRateMetric(t.MetricName));

                        return newMetric;
                    })
                .AsTupleList();
        }

        private static Metric GetHttpStatusCodeRateMetric(string httpStatusCodeMetricName)
        {
            var httpStatusCodeRateMetric = new Metric
            {
                Name = string.Concat(httpStatusCodeMetricName, MetricConstants.HttpStatusCodeBytesSentRateMetricNameSuffix),
                DisplayName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} {1} {2}",
                    MetricConstants.HttpStatusCodeMetricNamePrefix,
                    httpStatusCodeMetricName.Replace(MetricConstants.HttpStatusCodeMetricNamePrefix, string.Empty),
                    MetricConstants.HttpStatusCodeBytesSentRateMetricNameSuffix),
                Unit = MetricConstants.MegabitsPerSecondMetricUnit,
                DisplayUnit = MetricConstants.MegabitsPerSecondMetricDisplayUnit,
                AggregationType = MetricConstants.CurrentMetricAggregationType
            };

            return httpStatusCodeRateMetric;
        }
    }
}
