//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    /// <summary>
    /// Metrics for a Media Services Streaming Endpoint.
    /// </summary>
    public class StreamingEndpointMetrics
    {
        /// <summary>
        /// Initializes a new instance of the StreamingEndpointMetrics class.
        /// </summary>
        /// <param name="streamingEndpointRequestLogs">The collection of Streaming Endpoint request logs.</param>
        public StreamingEndpointMetrics(ICollection<StreamingEndpointRequestLog> streamingEndpointRequestLogs)
        {
            StreamingEndpointRequestLogs = streamingEndpointRequestLogs;
        }

        /// <summary>
        /// Gets the collection of Streaming Endpoint request logs.
        /// </summary>
        public ICollection<StreamingEndpointRequestLog> StreamingEndpointRequestLogs { get; }
    }
}