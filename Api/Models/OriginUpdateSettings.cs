namespace MediaDashboard.Operations.Api.Models
{
    public class OriginUpdateSettings
    {
        public string Description { get; set; }

        public IPRange[] AllowList { get; set; }
    }
}
