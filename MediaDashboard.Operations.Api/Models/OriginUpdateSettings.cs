namespace MediaDashboard.Web.Api.Models
{
    public class OriginUpdateSettings
    {
        public string Description { get; set; }

        public IPRange[] AllowList { get; set; }
    }
}
