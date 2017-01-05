using System;

namespace MediaDashboard.Common.Data
{

    public interface IMetricBase
    {
        string MediaServiceId { get; set; }
        DateTime TimeStamp { get; set; }
        
       /// <summary>
       /// Base Metric elements from SDK
       /// </summary>
        string AggregationType { get; set; }
        string DisplayName { get; set; }
        string Name { get; set; }
        string Unit { get; set; }
        long Value { get; set; }
    }
}
