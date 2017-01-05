using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class HttpStatusCodeRatioMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;

        public HttpStatusCodeRatioMetricCalculatorStrategy()
        {
            monitoringIntervalInSeconds = MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetry)
            where TCurrent : ITelemetry
        {
            var result = new TupleList<decimal, Metric>();


            var totalRequests = telemetry.FirstOrDefault(t => t.MetricName.Equals(MetricConstants.TotalRequestsMetricName, StringComparison.OrdinalIgnoreCase));
            if (totalRequests != null)
            {
                var totalRequestsCount = totalRequests.Value;

                result.AddRange(
                    telemetry
                    .Where(t => Regex.IsMatch(t.MetricName, MetricConstants.HttpStatusCodeReqeustsMetricRegularExpression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                        .Select(
                            t =>
                            {
                                decimal value = 0;
                                if (totalRequestsCount != 0)
                                {
                                    value = Math.Round(t.Value * 100/ totalRequestsCount, 3);
                                }

                                var newMetric = new Tuple<decimal, Metric>(
                                    value,
                                    GetHttpStatusCodeRatioMetric(t.MetricName));

                                return newMetric;
                            }));
            }

            return result;
        }

        private static Metric GetHttpStatusCodeRatioMetric(string httpStatusCodeMetricName)
        {
            var httpStatusCodeRatioMetric = new Metric
            {
                Name = string.Concat(httpStatusCodeMetricName, MetricConstants.HttpStatusCodeRatioMetricNameSuffix),
                DisplayName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} {1} {2}",
                    MetricConstants.HttpStatusCodeMetricNamePrefix,
                    httpStatusCodeMetricName.Replace(MetricConstants.HttpStatusCodeMetricNamePrefix, string.Empty),
                    MetricConstants.HttpStatusCodeRatioMetricNameSuffix),
                Unit = MetricConstants.RatioMetricUnit,
                DisplayUnit = MetricConstants.RatioMetricDisplayUnit,
                AggregationType = MetricConstants.CurrentMetricAggregationType
            };

            return httpStatusCodeRatioMetric;
        }
    }
}
