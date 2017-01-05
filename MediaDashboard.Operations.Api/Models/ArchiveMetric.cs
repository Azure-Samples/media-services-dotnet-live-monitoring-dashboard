using MediaDashboard.Common.Data;

namespace MediaDashboard.Web.Api.Models
{
    public class ArchiveMetricGroup
    {
        public string TrackType;

        public string TrackName;

        public string Bitrate;

        public HealthStatus Health;

        public ArchiveMetric[] Metrics;
    }

    public class ArchiveMetric
    {
        public string Name;

        public decimal Value;

        public HealthStatus Health;
    }
}