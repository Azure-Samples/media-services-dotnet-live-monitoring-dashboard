namespace MediaDashboard.Common.Metrics
{
    public static class MetricConstants
    {
        public const string MetricGroupIdSeparator = "<-|->";

        public const string MetricGroupIdDisplaySeparator = "/";

        public const string CurrentMetricAggregationType = "Current";

        public const string TotalMetricAggregationType = "Total";

        public const string AverageMetricAggregationType = "Weighted Average";

        public const string HttpStatusCodeReqeustsMetricRegularExpression = @"^Http\d{3}$";

        public const string FailedRequestsMetricRegularExpression = @"^Http[45]\d{2}";

        public const string FailedRequestsRatioMetricRegularExpression = @"^Http[45]\d{2}$";

        public const string Http412MetricName = "Http412";

        public const string Http412RatioMetricName = "Http412Ratio";

        public const string FailedRequestsMetricName = "FailedRequests";

        public const string FailedRequestsRatioMetricName = "FailedRequestsRatio";

        public const string FailedRequestBytesSent = "FailedRequestsBytesSent";

        public const string BitrateRatioMetricName = "BitrateRatio";

        public const string LastTimestampMetricName = "LastTimestamp";

        public const string DiscontinuityCountMetricName = "DiscontinutiyCount";

        public const string OverlapCountMetricName = "OverlapCount";

        public const string FragmentWaitTimeMetricName = "FragmentWaitTime";

        public const string BytesSentUtilizationRatioMetricName = "BytesSentUtilizationRatio";

        public const string BytesSentMetricName = "BytesSent";

        public const string BytesSentRateMetricName = "BytesSentRate";

        public const string EncodedBitrateMetricName = "EncodedBitrate";

        public const string IncomingBitrateMetricName = "IncomingBitrate";

        public const string TotalFragmentsDroppedMetricName = "TotalFragmentsDropped";

        public const string FragmentsDroppedRateMetricName = "FragmentsDroppedRate";

        public const string TotalFragmentsReceivedMetricName = "TotalFragmentsReceived";

        public const string TotalFragmentsUploadedMetricName = "TotalFragmentsUploaded";

        public const string FragmentsReceivedRateMetricName = "FragmentsReceivedRate";

        public const string FragmentsUploadedRateMetricName = "FragmentsUploadedRate";

        public const string FragmentsReceivedRatioMetricName = "FragmentsReceivedRatio";

        public const string FragmentsReceivedPerFragmentLengthMetricName = "FragmentsReceivedPerFragmentLength";

        public const string TotalRequestsMetricName = "TotalRequests";

        public const string RequestsRateMetricName = "Rate";

        public const string FailedRequessMetricDisplayName = "Failed Requests (4XX + 5XX - 412)";

        public const string FailedRequestsRatioMetricDisplayName = "Failed Requests Ratio (4XX + 5XX - 412)";

        public const string BitrateRatioMetricDisplayName = "Incoming / Encoded Bitrate";

        public const string TotalFragmentsDroppedMetricDisplayName = "Total Fragments Dropped";

        public const string FragmentsDroppedRateMetricDisplayName = "Fragments Dropped Rate";

        public const string FragmentsReceivedRateMetricDisplayName = "Fragments Received Rate";

        public const string FragmentsUploadedRateMetricDisplayName = "Fragments Uploaded Rate";

        public const string FragmentsReceivedRatioMetricDisplayName = "Fragments Received Ratio";

        public const string FragmentsReceivedPerFragmentLengthMetricDisplayName = "Fragments Received Per Fragment Length";

        public const string BytesSentUtilizationRatioMetricDisplayName = "Data Out Utilization Ratio";

        public const string BytesSentRateMetricDisplayName = "Data Out Rate";

        public const string RequestsRateMetricDisplayName = "Requests Rate";

        public const string RatioMetricUnit = "Ratio";

        public const string CountMetricUnit = "Count";

        public const string CountPerSecondMetricUnit = "CountPerSecond";

        public const string CountPerMinuteMetricUnit = "CountPerMinute";

        public const string CountPerFragmentLengthMetricUnit = "CountPerFragmentLength";

        public const string BytesMetricUnit = "Bytes";

        public const string MegabitsMetricUnit = "Megabits";

        public const string MillisecondsMetricUnit = "Milliseconds";

        public const string BitsPerSecondMetricUnit = "BitsPerSecond";

        public const string MegabitsPerSecondMetricUnit = "MegabitsPerSecond";

        public const string RatioMetricDisplayUnit = "ratio";

        public const string CountMetricDisplayUnit = "count";

        public const string CountPerSecondMetricDisplayUnit = "/sec";

        public const string CountPerMinuteMetricDisplayUnit = "/min";

        public const string CountPerPerFragmentLengthMetricDisplayUnit = "/frag size";

        public const string MillisecondsMetricDisplayUnit = "ms";

        public const string MegabitsMetricDisplayUnit = "Mbits";

        public const string MegabitsPerSecondMetricDisplayUnit = "Mbps";

        public const string HttpStatusCodeMetricNamePrefix = "Http";

        public const string HttpStatusCodeRateMetricNameSuffix = "Rate";

        public const string HttpStatusCodeRatioMetricNameSuffix = "Ratio";

        public const string HttpStatusCodeBytesSentMetricNameSuffix = "BytesSent";

        public const string HttpStatusCodeBytesSentRateMetricNameSuffix = "BytesSentRate";

        public const string HttpStatusCodeServerLatencyMetricNameSuffix = "ServerLatency";

        public const string HttpStatusCodeE2ELatencyMetricNameSuffix = "E2ELatency";

        public const int DefaultDefaultFragmentDurationInSeconds = 2;

        public const int DefaultMonitoringIntervalInSeconds = 30;

        //public const int DefaultOriginReservedUnitTheoreticalCapacityInMbps = App.Config.Parameters.ReservedUnit.Capacity;
    }
}
