//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.WindowsAzure.Storage.Table;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    /// <summary>
    /// A channel heartbeat metric.
    /// </summary>
    public class ChannelHeartbeat : TelemetryEvent
    {
        /// <summary>
        /// Gets the custom attributes.
        /// </summary>
        public string CustomAttributes { get; }

        /// <summary>
        /// Gets the track type.
        /// </summary>
        public string TrackType { get; }

        /// <summary>
        /// Gets the track name.
        /// </summary>
        public string TrackName { get; }

        /// <summary>
        /// Gets the bitrate.
        /// </summary>
        public int Bitrate { get; }

        /// <summary>
        /// Gets the incoming bitrate.
        /// </summary>
        public int IncomingBitrate { get; }

        /// <summary>
        /// Gets the overlap count.
        /// </summary>
        public int OverlapCount { get; }

        /// <summary>
        /// Gets the discontinuity count.
        /// </summary>
        public int DiscontinuityCount { get; }

        /// <summary>
        /// Gets the last time stamp.
        /// </summary>
        public ulong LastTimestamp { get; }

        /// <summary>
        /// Creates a ChannelHeartbeat object from a Azure Table Storage row.
        /// </summary>
        /// <param name="entity">The Azure Table Storage row.</param>
        /// <returns>The new ChannelHeartbeat object.</returns>
        internal ChannelHeartbeat(DynamicTableEntity entity):
            base(entity)
        {
            CustomAttributes = entity.Properties["CustomAttributes"].StringValue;
            TrackType = entity.Properties["TrackType"].StringValue;
            TrackName = entity.Properties["TrackName"].StringValue;
            Bitrate = entity.Properties["Bitrate"].Int32Value.GetValueOrDefault();
            IncomingBitrate = entity.Properties["IncomingBitrate"].Int32Value.GetValueOrDefault();
            OverlapCount = entity.Properties["OverlapCount"].Int32Value.GetValueOrDefault();
            DiscontinuityCount = entity.Properties["DiscontinuityCount"].Int32Value.GetValueOrDefault();
            LastTimestamp = (ulong)entity.Properties["LastTimestamp"].Int64Value.GetValueOrDefault();
        }
    }
}