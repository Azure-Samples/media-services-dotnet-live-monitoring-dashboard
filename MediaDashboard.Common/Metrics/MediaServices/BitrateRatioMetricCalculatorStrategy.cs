using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public class BitrateRatioMetricCalculatorStrategy : IMetricCalculatorStrategy
    {
        private Metric bitrateRatioMetric;

        public MetricType Type
        {
            get { return MetricType.Ingest; }
        }

        private Metric BitrateRatioMetric
        {
            get
            {
                if (bitrateRatioMetric == null)
                {
                    bitrateRatioMetric = new Metric
                    {
                        Name = MetricConstants.BitrateRatioMetricName,
                        DisplayName = MetricConstants.BitrateRatioMetricDisplayName,
                        Unit = MetricConstants.RatioMetricUnit,
                        DisplayUnit = MetricConstants.RatioMetricDisplayUnit,
                        AggregationType = MetricConstants.CurrentMetricAggregationType
                    };
                }

                return bitrateRatioMetric;
            }
        }

        public TupleList<decimal, Metric> CalculateMetrics<TCurrent>(List<TCurrent> telemetryTuples)
            where TCurrent : ITelemetry
        {
            var result = new TupleList<decimal, Metric>();

            var incomingBitrate = telemetryTuples.FirstOrDefault(t => t.MetricName.Equals(MetricConstants.IncomingBitrateMetricName, StringComparison.OrdinalIgnoreCase));
            if (incomingBitrate != null)
            {
                var encodedBitrate = telemetryTuples.FirstOrDefault(t => t.MetricName.Equals(MetricConstants.EncodedBitrateMetricName, StringComparison.OrdinalIgnoreCase));
                if (encodedBitrate != null)
                {
                    var currentIncomingBitrateValue = incomingBitrate.Value;
                    var currentEncodedBitrateValue = encodedBitrate.Value;

                    decimal value = (currentIncomingBitrateValue == 0) ? 0 : 1;
                    if (currentEncodedBitrateValue != 0)
                    {
                        value = Math.Round(Math.Abs(1 - (currentIncomingBitrateValue / currentEncodedBitrateValue)), 3);
                    }

                    var newMetric = new Tuple<decimal, Metric>(
                        value,
                        BitrateRatioMetric);

                    result.Add(newMetric);
                }
            }

            return result;
        }
    }
}
