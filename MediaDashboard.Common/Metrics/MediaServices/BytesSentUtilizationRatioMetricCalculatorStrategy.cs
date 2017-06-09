using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class BytesSentUtilizationRatio : IMetricCalculatorStrategy
    {
        private readonly int originReservedUnitTheoreticalCapacityInMbps;

        private readonly int monitoringIntervalInSeconds;
                
        private Metric bytesSentUtilizationRatioMetric;

        public BytesSentUtilizationRatio()
        {
            originReservedUnitTheoreticalCapacityInMbps = App.Config.Parameters.ReservedUnitConfig.Capacity;//MetricConstants.DefaultOriginReservedUnitTheoreticalCapacityInMbps;

            monitoringIntervalInSeconds = App.Config.Parameters.MonitoringIntervalConfig.DefaultIntervalSeconds;//MetricConstants.DefaultMonitoringIntervalInSeconds;
        }

        public MetricType Type
        {
            get { return MetricType.Origin; }
        }

        private Metric BytesSentUtilizationRatioMetric
        {
            get
            {
                if (this.bytesSentUtilizationRatioMetric == null)
                {
                    this.bytesSentUtilizationRatioMetric = new Metric
                    {
                        Name = MetricConstants.BytesSentUtilizationRatioMetricName,
                        DisplayName = MetricConstants.BytesSentUtilizationRatioMetricDisplayName,
                        Unit = MetricConstants.RatioMetricUnit,
                        DisplayUnit = MetricConstants.RatioMetricDisplayUnit,
                        AggregationType = MetricConstants.CurrentMetricAggregationType
                    };
                }

                return this.bytesSentUtilizationRatioMetric;
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            return telemetryTuples
                .Where(t => 
                    t.MetricName.Equals(MetricConstants.BytesSentMetricName, StringComparison.OrdinalIgnoreCase))
                .Select(
                    t =>
                    {
                        var currentRate = (t.Value) / monitoringIntervalInSeconds;

                        var originReservedUnits = (int)t.Miscellaneous;
                        var theoreticalCapacity = originReservedUnitTheoreticalCapacityInMbps * originReservedUnits;

                        if (theoreticalCapacity == 0) theoreticalCapacity = 3 * originReservedUnitTheoreticalCapacityInMbps; // for standard streaming endpoint

                        var newMetric = new Tuple<decimal, Metric>(
                            Math.Round((currentRate / theoreticalCapacity) * 100, 3),
                            BytesSentUtilizationRatioMetric);

                        return newMetric;
                    })
                .AsTupleList();
        }
    }
}
