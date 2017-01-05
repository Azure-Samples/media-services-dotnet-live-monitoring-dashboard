using MediaDashboard.Common.Data;

namespace MediaDashboard.Web.Api.Models
{
    public class IngestMetric
    {
        public string Name;

        public decimal Value;

        public HealthStatus Health;
    }

    public class IngestMetricGroup
    {
        public string TrackType;

        public string TrackName;

        public IngestMetric[] Metrics;

        public HealthStatus Health;
    }
}