using System.Collections.Generic;
using MediaDashboard.Common.Data;

namespace MediaDashboard.Common.Metrics.MediaServices
{
    public interface IMetricCalculator
    {
        IReadOnlyCollection<IMetricCalculatorStrategy> Strategies { get; }

        TupleList<IngestTelemetry, Metric> GenerateIngestMetrics(List<IngestTelemetry> ingestTelemetryTuples);

        TupleList<ProgramTelemetry, Metric> GenerateProgramMetrics(List<ProgramTelemetry> programTelemetryTuples);

        TupleList<EgressTelemetry, Metric> GenerateEgressMetrics(List<EgressTelemetry> egressTelemetryTuples);
    }
}
