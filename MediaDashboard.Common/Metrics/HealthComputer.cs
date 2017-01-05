using MediaDashboard.Common.Data;
using System.Globalization;

namespace MediaDashboard.Common.Metrics
{

    public static class HealthComputer
    {
        public static HealthMetricState Compute(ITelemetry telemetry)
        {
            if (telemetry.Metric == null || !telemetry.Metric.GreenBoundaryValue.HasValue || !telemetry.Metric.RedBoundaryValue.HasValue)
            {
                return new HealthMetricState(HealthStatus.Ignore);
            }

            var redBoundary = telemetry.Metric.RedBoundaryValue.Value;
            var greenBoundary = telemetry.Metric.GreenBoundaryValue.Value;

            var computer = NumericThreshold<decimal>.Create(greenBoundary, redBoundary);

            var level = computer.StateFor(telemetry.Value);

            return new HealthMetricState(
                level,
                telemetry.MetricName,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Computed thresholds for metric '{0}' with value '{1}' in '{2}'",
                    telemetry.MetricName,
                    telemetry.Value,
                    telemetry.GroupId.Replace(MetricConstants.MetricGroupIdSeparator, MetricConstants.MetricGroupIdDisplaySeparator)));
        }
    }
}