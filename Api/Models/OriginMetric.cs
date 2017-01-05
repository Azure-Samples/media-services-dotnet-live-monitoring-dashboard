using MediaDashboard.Common.Data;

namespace MediaDashboard.Operations.Api.Models
{
    public class OriginMetric
    {
        public string Name;

        public decimal Value;

        public HealthStatus Health;
    }

    public class OriginMetricGroup
    {
        public string Name;

        public HealthStatus Health;

        public OriginMetric[] Metrics;
    }
}