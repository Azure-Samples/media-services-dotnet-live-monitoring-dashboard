//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.WindowsAzure.Storage.Table;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    /// <summary>
    /// A Streaming Endpoint request log metric.
    /// </summary>
    public class StreamingEndpointRequestLog: TelemetryEvent
    {
        /// <summary>
        /// Gets the Streaming Endpoint host name.
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the result code.
        /// </summary>
        public string ResultCode { get; }

        /// <summary>
        /// Gets the request count.
        /// </summary>
        public int RequestCount { get; }

        /// <summary>
        /// Gets the bytes sent.
        /// </summary>
        public long BytesSent { get; }

        /// <summary>
        /// Gets the server latency.
        /// </summary>
        public int ServerLatency { get; }

        /// <summary>
        /// Gets the end to end request time.
        /// </summary>
        public int EndToEndLatency { get; }

        /// <summary>
        /// Creates a StreamingEndpointRequestLog object from a Azure Table Storage row.
        /// </summary>
        /// <param name="entity">The Azure Table Storage row.</param>
        /// <returns>The new StreamingEndpointRequestLog object.</returns>
        internal StreamingEndpointRequestLog(DynamicTableEntity entity):
            base(entity)
        {
            HostName = entity.Properties["HostName"].StringValue;
            StatusCode = entity.Properties["StatusCode"].Int32Value.GetValueOrDefault();
            ResultCode = entity.Properties["ResultCode"].StringValue;
            RequestCount = entity.Properties["RequestCount"].Int32Value.GetValueOrDefault();
            BytesSent = entity.Properties["BytesSent"].Int64Value.GetValueOrDefault();
            ServerLatency = entity.Properties["ServerLatency"].Int32Value.GetValueOrDefault();
            EndToEndLatency = entity.Properties["E2ELatency"].Int32Value.GetValueOrDefault();
        }

        public StreamingEndpointRequestLog(StreamingEndpointRequestLog other, int statusCode) : base(other)
        {
            HostName = other.HostName;
            StatusCode = statusCode;
            RequestCount = 0;
            BytesSent = 0;
            ServerLatency = 0;
            EndToEndLatency = 0;
        }
    }
}