using System;

namespace MediaDashboard.Common.Data
{
    public class ArchiveMetric : IMetricBase
    {
        public string CachKey
        {
            get { return string.Format("{0}-{1}", ProgramId, Name); }
        }

        public string ProgramId { get; set; }
        // public string ProgramName { get; set; }  // No longer present in SDK

        public string StreamId { get; set; }
        public int TrackId { get; set; }
        public string TrackName { get; set; }

        /// <summary>
        /// From IMetricBase
        /// </summary>
        ///         public string MediaServiceId { get; set; }
        public string MediaServiceId { get; set; }
        public DateTime TimeStamp { get; set; }

        public string AggregationType { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public long Value { get; set; }
    }
}
