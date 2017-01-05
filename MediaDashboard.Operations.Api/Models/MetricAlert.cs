using System;

namespace MediaDashboard.Web.Api.Models
{
    public class MetricAlert
    {
        public int AlertID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
