using MediaDashboard.Common.Data;

namespace MediaDashboard.Common.Metrics
{
    public class HealthMetricState
    {
        public HealthMetricState(HealthStatus level)
            : this(level, null, null)
        {
        }

        public HealthMetricState(HealthStatus level, string details)
            : this(level, null, details)
        {
        }

        public HealthMetricState(HealthStatus level, string metric, string details)
        {
            this.Level = level;
            this.MetricName = metric;
            this.Details = details;
        }

        public HealthStatus Level { get; set; }

        public string MetricName { get; set; }
        public decimal MetricValue { get; set; }
        public string Details { get; set; }
    }
}
