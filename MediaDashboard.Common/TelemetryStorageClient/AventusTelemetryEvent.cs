using Microsoft.WindowsAzure.Storage.Table;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    public class AventusTelemetryEvent : TelemetryEvent
    {
        public string Data { get; }

        internal AventusTelemetryEvent(DynamicTableEntity entity):
            base(entity)
        {
            Data = entity.Properties["Data"].StringValue;
        }
    }
}
