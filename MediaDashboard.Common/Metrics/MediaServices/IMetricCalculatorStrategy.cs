using MediaDashboard.Common.Data;
using System.Collections.Generic;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public interface IMetricCalculatorStrategy
    {
        MetricType Type { get; }

        TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetry)
            where TCurrent : ITelemetry;
    }
}
