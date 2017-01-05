using MediaDashboard.Common.Data;
using System.Globalization;

namespace MediaDashboard.Common.Metrics
{
    public class ProgramTelemetry : ITelemetry
    {
        public string ProgramId { get; set; }

        public string StreamId { get; set; }

        public string TrackId { get; set; }

        public string Bitrate { get; set; }

        public string VirtualPath { get; set; }

        public string MetricName { get; set; }
        public System.DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    
        public virtual Metric Metric { get; set; }

        public string GroupId
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{4}{1}{4}{2}{4}{3}",
                    ProgramId,
                    StreamId,
                    TrackId,
                    Bitrate,
                    MetricConstants.MetricGroupIdSeparator);
            }
        }

        public object Miscellaneous { get; set; }

        public HealthMetricState ComputeHealthState()
        {
            return HealthComputer.Compute(this);
        }
    }
}
