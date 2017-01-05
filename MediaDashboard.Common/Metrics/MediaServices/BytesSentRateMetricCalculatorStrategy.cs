using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class BytesSentRateMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;

        private Metric bytesSentRateMetric;

        public BytesSentRateMetricCalculatorStrategy()
        {
            monitoringIntervalInSeconds = MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        private Metric BytesSentRateMetric
        {
            get
            {
                if (bytesSentRateMetric == null)
                {
                    bytesSentRateMetric = new Metric
                    {
                        Name = MetricConstants.BytesSentRateMetricName,
                        DisplayName = MetricConstants.BytesSentRateMetricDisplayName,
                        Unit = MetricConstants.MegabitsPerSecondMetricUnit,
                        DisplayUnit = MetricConstants.MegabitsPerSecondMetricDisplayUnit,
                        AggregationType = MetricConstants.CurrentMetricAggregationType
                    };
                }

                return bytesSentRateMetric;
            }
        }

        private Metric FailedRequestBytesSentRateMetric
        {
            get
            {
                return new Metric
                {
                    Name = MetricConstants.FailedRequestsMetricName + MetricConstants.BytesSentRateMetricName,
                    DisplayName = MetricConstants.FailedRequestsMetricName + MetricConstants.BytesSentRateMetricDisplayName,
                    Unit = MetricConstants.MegabitsPerSecondMetricUnit,
                    DisplayUnit = MetricConstants.MegabitsPerSecondMetricDisplayUnit,
                    AggregationType = MetricConstants.CurrentMetricAggregationType
                };
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            return telemetryTuples
                .Where(t => t.MetricName.Equals(MetricConstants.BytesSentMetricName, StringComparison.OrdinalIgnoreCase) )
                .Select(
                    t =>
                    {
                        var newMetric = new Tuple<decimal, Metric>(
                            Math.Round((t.Value) / monitoringIntervalInSeconds, 3),
                            BytesSentRateMetric);

                        return newMetric;
                    })
                .AsTupleList();
        }
    }
}
