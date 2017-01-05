using MediaDashboard.Common.Data;
using System;

namespace MediaDashboard.Common.Metrics
{
    public interface ITelemetry
    {
        string GroupId { get; }

        string MetricName { get; set; }
        
        DateTime Timestamp { get; set; }

        decimal Value { get; set; }

        Metric Metric { get; set; }

        object Miscellaneous { get; set; }

        HealthMetricState ComputeHealthState();
    }
}
