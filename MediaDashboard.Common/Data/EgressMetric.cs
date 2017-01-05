using System;

namespace MediaDashboard.Common.Data
{
    public class EgressMetric : IMetricBase
    {
        public string CachKey
        {
            get { return string.Format("{0}-{1}", OrginId, Name); }
        }

        public string OrginId { get; set; }
        public string OriginName { get; set; }

        /// <summary>
        /// From IMetricBase
        /// </summary>
        public string MediaServiceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string AggregationType { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public long Value { get; set; }
    }
}
