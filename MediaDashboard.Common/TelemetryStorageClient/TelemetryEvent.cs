using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    public abstract class TelemetryEvent
    {
        /// <summary>
        /// Gets the partition key of the record.
        /// </summary>
        public string PartitionKey { get; }

        /// <summary>
        /// Gets the row key of the record.
        /// </summary>
        public string RowKey { get; }

        /// <summary>
        /// Gets the Media Services account ID.
        /// </summary>
        public Guid AccountId { get; }

        /// <summary>
        /// Gets the Media Services Channel ID.
        /// </summary>
        public Guid EntityId { get; }

        /// <summary>
        /// Gets the observed time of the metric.
        /// </summary>
        public DateTime ObservedTime { get; }

        /// <summary>
        /// The type of the telemetry event.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The name of the telemetry event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a ChannelHeartbeat object from a Azure Table Storage row.
        /// </summary>
        /// <param name="entity">The Azure Table Storage row.</param>
        /// <returns>The new ChannelHeartbeat object.</returns>
        internal TelemetryEvent(DynamicTableEntity entity)
        {
            var partitionKeyParts = entity.PartitionKey.Split('_');

            PartitionKey = entity.PartitionKey;
            RowKey = entity.RowKey;
            AccountId = Guid.ParseExact(partitionKeyParts[0], "N");
            if(entity.Properties.ContainsKey("ServiceId"))
            {
                //  right now null for aventus events so check needed..
                EntityId = entity.Properties["ServiceId"].GuidValue.GetValueOrDefault();
            }
            ObservedTime = entity.Properties["ObservedTime"].DateTime.GetValueOrDefault();
            Type = entity.Properties["Type"].StringValue;
            Name = entity.Properties["Name"].StringValue;
        }

        protected TelemetryEvent(TelemetryEvent other)
        {
            EntityId = other.EntityId;
            AccountId = other.AccountId;
            PartitionKey = other.PartitionKey;
            RowKey = other.RowKey;
            Name = other.Name;
            Type = other.Type;
            ObservedTime = other.ObservedTime;
        }

    }
}
