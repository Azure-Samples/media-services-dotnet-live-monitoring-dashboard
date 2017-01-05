using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class FailedRequestsRatioMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;

        private Metric failedRequestsRatioMetric;

        public FailedRequestsRatioMetricCalculatorStrategy()
        {
            monitoringIntervalInSeconds = MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        private Metric FailedRequestsRatioMetric
        {
            get
            {
                if (failedRequestsRatioMetric == null)
                {
                    failedRequestsRatioMetric = new Metric
                    {
                        Name = MetricConstants.FailedRequestsRatioMetricName,
                        DisplayName = MetricConstants.FailedRequestsRatioMetricDisplayName,
                        Unit = MetricConstants.RatioMetricUnit,
                        DisplayUnit = MetricConstants.RatioMetricDisplayUnit,
                        AggregationType = MetricConstants.CurrentMetricAggregationType
                    };
                }

                return failedRequestsRatioMetric;
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            var result = new TupleList<decimal, Metric>();

            var totalRequests = telemetryTuples.FirstOrDefault(t => t.MetricName.Equals(MetricConstants.TotalRequestsMetricName, StringComparison.OrdinalIgnoreCase));
            var failedRequests = telemetryTuples.FirstOrDefault(t => t.MetricName.Equals(MetricConstants.FailedRequestsMetricName, StringComparison.OrdinalIgnoreCase));

            if (totalRequests != null && failedRequests != null)
            {
                var value = (totalRequests.Value > 0) ? Math.Round(failedRequests.Value / totalRequests.Value, 3) : 0;
                var newMetric = new Tuple<decimal, Metric>(
                    value,
                    FailedRequestsRatioMetric);

                result.Add(newMetric);
            }

            return result;
        }
    }
}
