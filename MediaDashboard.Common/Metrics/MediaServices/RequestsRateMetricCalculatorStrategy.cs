using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{

    public class RequestsRateMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;
        
        private Metric totalRequestsRateMetric;
                
        public RequestsRateMetricCalculatorStrategy()
        {
            monitoringIntervalInSeconds = MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        private Metric TotalRequestsRateMetric
        {
            get
            {
                if (totalRequestsRateMetric == null)
                {
                    totalRequestsRateMetric = new Metric
                    {
                        Name = MetricConstants.TotalRequestsMetricName + MetricConstants.RequestsRateMetricName,
                        DisplayName = MetricConstants.RequestsRateMetricDisplayName,
                        Unit = MetricConstants.CountPerMinuteMetricUnit,
                        DisplayUnit = MetricConstants.CountPerMinuteMetricDisplayUnit,
                        AggregationType = MetricConstants.CurrentMetricAggregationType
                    };
                }

                return totalRequestsRateMetric;
            }
        }

        private Metric FailedRequestsRateMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.FailedRequestsMetricName + MetricConstants.RequestsRateMetricName,
                    DisplayName = MetricConstants.FailedRequestsMetricName + MetricConstants.RequestsRateMetricDisplayName,
                    Unit = MetricConstants.CountPerMinuteMetricUnit,
                    DisplayUnit = MetricConstants.CountPerMinuteMetricDisplayUnit,
                    AggregationType = MetricConstants.CurrentMetricAggregationType
                };
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            var result = new TupleList<decimal, Metric>();

            var totalRequestRate = telemetryTuples
                .Where(t => t.MetricName.Equals(MetricConstants.TotalRequestsMetricName, StringComparison.OrdinalIgnoreCase))
                .Select(
                    t =>
                    {
                        var newMetric = new Tuple<decimal, Metric>(
                            Math.Round((t.Value * 60) / monitoringIntervalInSeconds, 3),
                            TotalRequestsRateMetric);

                        return newMetric;
                    });
            result.AddRange(totalRequestRate);

            var failedRequestRate = telemetryTuples
                            .Where(t => t.MetricName.Equals(MetricConstants.FailedRequestsMetricName, StringComparison.OrdinalIgnoreCase))
                            .Select(
                                t =>
                                {
                                    var newMetric = new Tuple<decimal, Metric>(
                                        Math.Round((t.Value * 60) / monitoringIntervalInSeconds, 3),
                                        FailedRequestsRateMetric);

                                    return newMetric;
                                });
            result.AddRange(failedRequestRate);

            return result;
        }
    }
}
