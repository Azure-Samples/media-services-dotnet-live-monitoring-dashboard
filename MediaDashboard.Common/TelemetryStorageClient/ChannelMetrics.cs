//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    /// <summary>
    /// Metrics for a Media Services Channel.
    /// </summary>
    public class ChannelMetrics
    {
        /// <summary>
        /// Initializes a new instance of the ChannelMetrics class.
        /// </summary>
        /// <param name="channelHeartbeats">The collection of channel heartbeats.</param>
        public ChannelMetrics(ICollection<ChannelHeartbeat> channelHeartbeats,
            ICollection<AventusTelemetryEvent> aventusTelemetry,
            ICollection<ChannelFragmentDiscarded> channelFragmentsDiscarded)
        {
            ChannelHeartbeats = channelHeartbeats;
            AventusTelemetry = aventusTelemetry;
            ChannelFragmentsDiscarded = channelFragmentsDiscarded;
        }

        /// <summary>
        /// Gets the collection of channel heartbeats.
        /// </summary>
        public ICollection<ChannelHeartbeat> ChannelHeartbeats { get; }

        public ICollection<AventusTelemetryEvent> AventusTelemetry { get; }

        public ICollection<ChannelFragmentDiscarded> ChannelFragmentsDiscarded { get; }
    }
}