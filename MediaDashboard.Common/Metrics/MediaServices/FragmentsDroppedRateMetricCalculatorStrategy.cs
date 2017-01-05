using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class FragmentsDroppedRateMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private readonly int monitoringIntervalInSeconds;

        private Metric fragmentsDroppedRateMetric;

        public FragmentsDroppedRateMetricCalculatorStrategy()
        {
            monitoringIntervalInSeconds = MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Archive; }
        }

        private Metric FragmentsDroppedRateMetric
        {
            get
            {
                if (this.fragmentsDroppedRateMetric == null)
                {
                    this.fragmentsDroppedRateMetric = new Metric
                    {
                        Name = MetricConstants.FragmentsDroppedRateMetricName,
                        DisplayName = MetricConstants.FragmentsDroppedRateMetricDisplayName,
                        Unit = MetricConstants.CountPerMinuteMetricUnit,
                        DisplayUnit = MetricConstants.CountPerMinuteMetricDisplayUnit,
                        AggregationType = MetricConstants.CurrentMetricAggregationType
                    };
                }

                return this.fragmentsDroppedRateMetric;
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            return telemetryTuples
                .Where(t => t.MetricName.Equals(MetricConstants.TotalFragmentsDroppedMetricName, StringComparison.OrdinalIgnoreCase))
                .Select(
                    t =>
                    {
                        var newMetric = new Tuple<decimal, Metric>(
                            Math.Round((t.Value) / monitoringIntervalInSeconds * 60, 3),
                            FragmentsDroppedRateMetric);

                        return newMetric;
                    })
                .AsTupleList();
        }
    }
}
