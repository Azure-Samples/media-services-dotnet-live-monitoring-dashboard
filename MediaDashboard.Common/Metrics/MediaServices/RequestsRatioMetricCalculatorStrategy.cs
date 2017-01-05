using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{

    public class RequestsRatioMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;

        private Metric totalRequestsRateMetric;

        public RequestsRatioMetricCalculatorStrategy()
        {
            monitoringIntervalInSeconds = MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        private Metric TotalRequestsRatioMetric
        {
            get
            {
                if (totalRequestsRateMetric == null)
                {
                    totalRequestsRateMetric = new Metric
                    {
                        Name = MetricConstants.TotalRequestsMetricName + MetricConstants.HttpStatusCodeRatioMetricNameSuffix,
                        DisplayName = MetricConstants.RequestsRateMetricDisplayName,
                        Unit = MetricConstants.RatioMetricUnit,
                        DisplayUnit = MetricConstants.RatioMetricDisplayUnit,
                        AggregationType = MetricConstants.CurrentMetricAggregationType
                    };
                }

                return totalRequestsRateMetric;
            }
        }

        private Metric FailedRequestsRatioMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.FailedRequestsMetricName + MetricConstants.HttpStatusCodeRatioMetricNameSuffix,
                    DisplayName = MetricConstants.FailedRequestsMetricName + MetricConstants.RequestsRateMetricDisplayName,
                    Unit = MetricConstants.RatioMetricUnit,
                    DisplayUnit = MetricConstants.RatioMetricDisplayUnit,
                    AggregationType = MetricConstants.CurrentMetricAggregationType
                };
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            var result = new TupleList<decimal, Metric>();

            var totalRequests = telemetryTuples
                .Where(t => t.MetricName.Equals(MetricConstants.TotalRequestsMetricName, StringComparison.OrdinalIgnoreCase))
                .Sum(t => t.Value);

            var totalRequestRatio = new Tuple<decimal, Metric>(100m, TotalRequestsRatioMetric);
            result.Add(totalRequestRatio);

            var failedRequests = telemetryTuples
                            .Where(t => t.MetricName.Equals(MetricConstants.FailedRequestsMetricName, StringComparison.OrdinalIgnoreCase))
                            .Sum(t => t.Value);

            decimal failedRequestsRatio = 0m;
            if (totalRequests != 0)
            {
                failedRequestsRatio = Math.Round(failedRequests * 100/ totalRequests, 3);
            }
            result.Add(new Tuple<decimal, Metric>(failedRequestsRatio, FailedRequestsRatioMetric));

            return result;
        }
    }
}
