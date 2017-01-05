//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using MediaDashboard.Common.Config.Entities;
using Microsoft.WindowsAzure.Storage;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    /// <summary>
    /// A client for reading metrics written to Azure Table Storage by the Media Services Telemetry service.
    /// </summary>
    public class TelemetryStorage
    {
        private const int SecondsPerDay = 86400;
        private const int ApproximateChannelEventsPerMinute = 15;
        private const int ApproximateStreamingEndpointEventsPerMinute = 10;
        private const int TimeSkewMinutes = 5;
        private const string EventsTablePrefix = "TelemetryEvents";
        private const string MetricsTablePrefix = "TelemetryMetrics";
        private readonly CloudTableClient _tableClient;

        public TelemetryStorage(StorageAccountConfig storageConfig)
        {
            var credentials = new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey);
            var storage = new CloudStorageAccount(credentials, true);
            _tableClient = storage.CreateCloudTableClient();
        }

        /// <summary>
        /// Initializes a new instance of the TelemetryStorage class.
        /// </summary>
        /// <param name="storageCredentials">Storage credentials for the storage account containing the telemetry data.</param>
        /// <param name="tableEndpoint">The table endpoint of the storage account.</param>
        public TelemetryStorage(StorageCredentials storageCredentials, Uri tableEndpoint)
        {
            _tableClient = new CloudTableClient(tableEndpoint, storageCredentials);
        }

        /// <summary>
        /// Gets metrics for a Media Services Channel.
        /// </summary>
        /// <param name="accountId">The Media Services account ID.</param>
        /// <param name="channelId">The Channel ID.</param>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        /// <returns>The Channel metrics for the given channel within the given time range.</returns>
        public ChannelMetrics GetChannelMetrics(Guid accountId, Guid channelId, DateTime start, DateTime end)
        {
            return GetMetrics(accountId, channelId, start, end, CreateChannelMetrics);
        }

        /// <summary>
        /// Gets recent metrics for a Media Services Channel.
        /// </summary>
        /// <param name="accountId">The Media Services account ID.</param>
        /// <param name="channelId">The Channel ID.</param>
        /// <param name="approximateCount">The approximate count of items to return.</param>
        /// <returns>The Channel metrics.</returns>
        public ChannelMetrics GetRecentChannelMetrics(Guid accountId, Guid channelId, int approximateCount)
        {

            // Need exception handling here because if you put in the wrong storage account in the config.json
            // it silently fails and is not intuitive what is wrong.

            var queries = CreateQueries(MetricsTablePrefix, accountId, channelId, approximateCount, ApproximateChannelEventsPerMinute);
            Predicate<DynamicTableEntity> predicate = entity => true; // include all results
            return CreateChannelMetrics(queries, predicate);
        }

        /// <summary>
        /// Gets metrics for a Media Services Streaming Endpoint.
        /// </summary>
        /// <param name="accountId">The Media Services account ID.</param>
        /// <param name="streamingEndpointId">The Streaming Endpoint ID.</param>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        /// <returns>The Streaming Endpoint metrics for the given channel within the given time range.</returns>
        public StreamingEndpointMetrics GetStreamingEndpointMetrics(Guid accountId, Guid streamingEndpointId, DateTime start, DateTime end)
        {
            return GetMetrics(accountId, streamingEndpointId, start, end, CreateStreamingEndpointMetrics);
        }

        /// <summary>
        /// Gets recent metrics for a Media Services Streaming Endpoint.
        /// </summary>
        /// <param name="accountId">The Media Services account ID.</param>
        /// <param name="streamingEndpointId">The Streaming Endpoint ID.</param>
        /// <param name="approximateCount">The approximate count of items to return.</param>
        /// <returns>The Streaming Endpoint metrics.</returns>
        public StreamingEndpointMetrics GetRecentStreamingEndpointMetrics(Guid accountId, Guid streamingEndpointId, int approximateCount)
        {
            var queries = CreateQueries(MetricsTablePrefix, accountId, streamingEndpointId, approximateCount, ApproximateStreamingEndpointEventsPerMinute);
            Predicate<DynamicTableEntity> predicate = entity => true; // include all results
            return CreateStreamingEndpointMetrics(queries, predicate);
        }

        private static ChannelMetrics CreateChannelMetrics(IEnumerable<IQueryable<DynamicTableEntity>> queries, Predicate<DynamicTableEntity> predicate)
        {
            var channelHeartbeats = new List<ChannelHeartbeat>();
            var aventusTelemetry = new List<AventusTelemetryEvent>();
            var fragmentsDiscarded = new List<ChannelFragmentDiscarded>();
            // Execute each of the queries (this could be executed in parallel if needed).
            foreach (var query in queries)
            {
                foreach (var item in query.SkipTableNotFoundErrors())
                {
                    var itemName = item.Properties["Name"].StringValue;

                    if (!predicate(item))
                    {
                        continue;
                    }

                    // Parse the items and them to the result collections.
                    switch (itemName)
                    {
                        case "ChannelHeartbeat":
                            channelHeartbeats.Add(new ChannelHeartbeat(item));
                            break;
                        case "AventusTelemetry":
                            aventusTelemetry.Add(new AventusTelemetryEvent(item));
                            break;
                        case "ChannelFragmentDiscarded":
                            fragmentsDiscarded.Add(new ChannelFragmentDiscarded(item));
                            break;
                    }
                }
            }

            return new ChannelMetrics(channelHeartbeats, aventusTelemetry, fragmentsDiscarded);
        }

        private static StreamingEndpointMetrics CreateStreamingEndpointMetrics(IEnumerable<IQueryable<DynamicTableEntity>> queries, Predicate<DynamicTableEntity> predicate)
        {
            var streamingEndpointRequestLog = new List<StreamingEndpointRequestLog>();

            // Execute each of the queries (this could be executed in parallel if needed).
            foreach (var query in queries)
            {
                foreach (var item in query.SkipTableNotFoundErrors())
                {
                    var itemName = item.Properties["Name"].StringValue;

                    if (!predicate(item))
                    {
                        continue;
                    }

                    // Parse the items and them to the result collections.
                    switch (itemName)
                    {
                        case "StreamingEndpointRequestLog":
                            streamingEndpointRequestLog.Add(new StreamingEndpointRequestLog(item));
                            break;
                    }
                }
            }

            return new StreamingEndpointMetrics(streamingEndpointRequestLog);
        }

        private static IQueryable<DynamicTableEntity> CreateQuery(CloudTable table, Guid accountId, Guid serviceId, int count)
        {
            var partitionKey = string.Join("_", accountId.ToString("n"), serviceId.ToString("n"));

            return table
                .CreateQuery<DynamicTableEntity>()
                .Where(x => x.PartitionKey == partitionKey)
                .Take(count);
        }

        private static string FormatTableName(string tablePrefix, DateTime date)
        {
            return tablePrefix + date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        private T GetMetrics<T>(
            Guid accountId,
            Guid serviceId,
            DateTime start,
            DateTime end,
            Func<IEnumerable<IQueryable<DynamicTableEntity>>, Predicate<DynamicTableEntity>, T> createFunc)
        {
            if (start.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("'start' must be a UTC date and time.", "start");
            }

            if (end.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("'end' must be a UTC date and time.", "end");
            }

            if (start >= end)
            {
                throw new ArgumentException("Start time must be before the end time.", "start");
            }

            // Expand the query range to allow for telemetry batching and latency (the result will later be filtered to the requested
            // range.
            var queryStart = start.AddMinutes(-TimeSkewMinutes);
            var queryEnd = end.AddMinutes(TimeSkewMinutes);

            // Create queries for each day in the query range.
            var queries = CreateQueries(MetricsTablePrefix, accountId, serviceId, queryStart, queryEnd);

            // Drop items outside of the query range.
            Predicate<DynamicTableEntity> filter = entity =>
            {
                var observedTime = entity.Properties["ObservedTime"].DateTime.GetValueOrDefault();
                return observedTime > start && observedTime < end;
            };

            return createFunc(queries, filter);
        }

        private IEnumerable<IQueryable<DynamicTableEntity>> CreateQueries(string tablePrefix, Guid accountId, Guid serviceId, DateTime queryStart, DateTime queryEnd)
        {
            var dates = Enumerable.Range(0, (queryEnd.Date - queryStart.Date).Days + 1).Select(x => queryStart.AddDays(x).Date).ToArray();
            var partitionKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", accountId.ToString("n"), serviceId.ToString("n"));
            var startMinRowKey = string.Format(CultureInfo.InvariantCulture, "{0:D5}_99999", SecondsPerDay - (int)queryStart.TimeOfDay.TotalSeconds);
            var endMaxRowKey = string.Format(CultureInfo.InvariantCulture, "{0:D5}_00000", SecondsPerDay - (int)queryEnd.TimeOfDay.TotalSeconds);

            return dates.Select((date, index) =>
            {
                var table = _tableClient.GetTableReference(FormatTableName(tablePrefix, date));
                var startOfRange = index == 0;
                var endOfRange = index == dates.Length - 1;

                if (startOfRange && endOfRange)
                {
                    // For dates that contain both the start and end of the query range (i.e. the query does not cross any UTC
                    // day boundaries), give a range of row keys within the partition.
                    return table
                        .CreateQuery<DynamicTableEntity>()
                        .Where(x =>
                            x.PartitionKey == partitionKey &&
                            string.Compare(x.RowKey, startMinRowKey, StringComparison.Ordinal) < 0 &&
                            string.Compare(x.RowKey, endMaxRowKey, StringComparison.Ordinal) > 0);
                }
                else if (startOfRange)
                {
                    // For dates that contain the start of the query range (i.e. the query starts on this date and continues to the
                    // next day), specify the minimum row key.
                    return table
                        .CreateQuery<DynamicTableEntity>()
                        .Where(x =>
                            x.PartitionKey == partitionKey &&
                            string.Compare(x.RowKey, startMinRowKey, StringComparison.Ordinal) < 0);
                }
                else if (endOfRange)
                {
                    // For dates that contain the end of the query range (i.e. the query starts on a previous day and continues through
                    // part of this day), specify the maximum row key.
                    return table
                        .CreateQuery<DynamicTableEntity>()
                        .Where(x =>
                            x.PartitionKey == partitionKey &&
                            string.Compare(x.RowKey, endMaxRowKey, StringComparison.Ordinal) > 0);
                }
                else
                {
                    // For dates where the query does not start or end (i.e. whole days within the query), filter only on the
                    // partition key.
                    return table
                        .CreateQuery<DynamicTableEntity>()
                        .Where(x => x.PartitionKey == partitionKey);
                }
            });
        }

        private IEnumerable<IQueryable<DynamicTableEntity>> CreateQueries(string tablePrefix, Guid accountId, Guid serviceId, int count, int approximateEventsPerMinute)
        {
            var lookBackMinutes = count / approximateEventsPerMinute;

            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMinutes(-lookBackMinutes);

            if (startDate.Date == endDate.Date)
            {
                var table = _tableClient.GetTableReference(FormatTableName(tablePrefix, startDate));
                return new[] { CreateQuery(table, accountId, serviceId, count) };
            }
            else
            {
                return CreateQueries(tablePrefix, accountId, serviceId, startDate.AddMinutes(-TimeSkewMinutes), endDate.AddMinutes(TimeSkewMinutes));
            }
        }
    }
}
