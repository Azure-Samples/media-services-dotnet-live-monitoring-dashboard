using System;

namespace MediaDashboard.Common.Data
{
    // An enum for the type of metric.
    [Flags]
    public enum MetricType
    {
        Archive = 1,
        Encoding = 2,
        Ingest = 4,
        Origin = 8,
        Database = 12,
        Account = 14,
        None = 16
    }
}
