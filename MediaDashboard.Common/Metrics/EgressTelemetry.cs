using MediaDashboard.Common.Data;
using System;

namespace MediaDashboard.Common.Metrics
{
    public class EgressTelemetry : ITelemetry
    {
        public string OriginId { get; set; }
        public string MetricName { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    
        public object Miscellaneous { get; set; }
        public virtual Metric Metric { get; set; }

        public string GroupId { get { return OriginId; } }

        public HealthMetricState ComputeHealthState()
        {
            return HealthComputer.Compute(this);
        }
    }
}
