namespace MediaDashboard.Operations.Api.Models
{
    public class ChannelUpdateSettings
    {
        public string Description { get; set; }

        public IPRange[] IngestAllowList;

        public IPRange[] PreviewAllowList;
    }
}
