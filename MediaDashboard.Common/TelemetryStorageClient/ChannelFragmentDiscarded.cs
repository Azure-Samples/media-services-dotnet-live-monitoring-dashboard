using Microsoft.WindowsAzure.Storage.Table;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    public class ChannelFragmentDiscarded : TelemetryEvent
    {
        public string VirtualPath { get; }

        public string  TrackName { get; }

        public int Bitrate { get; }

        public int Count { get;}


        internal ChannelFragmentDiscarded(DynamicTableEntity entity):base(entity)
        {

        }
    }
}
