namespace MediaDashboard.Models
{
    public class ChannelUpdateSettings
    {
        public string Description { get; set; }

        public IPRange[] IngestAllowList;

        public IPRange[] PreviewAllowList;
    }
}
