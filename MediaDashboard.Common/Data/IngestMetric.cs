using System;

namespace MediaDashboard.Common.Data
{
    public class IngestMetric : IMetricBase
    {
        public string CachKey
        {
            get { return string.Format("{0}-{1}-{2}-{3}", ChannelId, Name, StreamId, TrackId); }
        }

        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
    
        // Appended from StreamMetrics
        public string StreamId { get; set; }
        public int TrackId { get; set; }
        public string TrackName { get; set; }


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
