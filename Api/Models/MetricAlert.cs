using System;

namespace MediaDashboard.Operations.Api.Models
{
    public class MetricAlert
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
