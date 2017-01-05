using MediaDashboard.Common.Data;
using System;

namespace MediaDashboard.Web.Api.Models
{
    public class AlertsQuery
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public MetricType[] MetricTypes { get; set; }

        public HealthStatus[] StatusLevels { get; set; }
    }
}