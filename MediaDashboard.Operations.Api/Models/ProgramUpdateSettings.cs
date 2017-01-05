using System;

namespace MediaDashboard.Web.Api.Models
{
    public class ProgramUpdateSettings
    {
        public string Description { get; set; }

        public TimeSpan ArchiveWindowLength { get; set; }
    }
}
