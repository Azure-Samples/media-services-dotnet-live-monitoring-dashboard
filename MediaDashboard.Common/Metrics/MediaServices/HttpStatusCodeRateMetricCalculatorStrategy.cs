using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class HttpStatusCodeRateMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;

        public HttpStatusCodeRateMetricCalculatorStrategy()
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
            return telemetryTuples
                    .Where(t => Regex.IsMatch(t.MetricName, MetricConstants.HttpStatusCodeReqeustsMetricRegularExpression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                    .Select(t =>
                    {
                        var newMetric = new Tuple<decimal, Metric>(
                            Math.Round(t.Value * 60 / monitoringIntervalInSeconds, 3),
                            GetHttpStatusCodeRateMetric(t.MetricName));

                        return newMetric;
                    })
                .AsTupleList();
        }

        private static Metric GetHttpStatusCodeRateMetric(string httpStatusCodeMetricName)
        {
            var httpStatusCodeRateMetric = new Metric
            {
                Name = string.Concat(httpStatusCodeMetricName, MetricConstants.HttpStatusCodeRateMetricNameSuffix),
                DisplayName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} {1} {2}",
                    MetricConstants.HttpStatusCodeMetricNamePrefix,
                    httpStatusCodeMetricName.Replace(MetricConstants.HttpStatusCodeMetricNamePrefix, string.Empty),
                    MetricConstants.HttpStatusCodeRateMetricNameSuffix),
                Unit = MetricConstants.CountPerMinuteMetricUnit,
                DisplayUnit = MetricConstants.CountPerMinuteMetricDisplayUnit,
                AggregationType = MetricConstants.CurrentMetricAggregationType
            };

            return httpStatusCodeRateMetric;
        }
    }
}
