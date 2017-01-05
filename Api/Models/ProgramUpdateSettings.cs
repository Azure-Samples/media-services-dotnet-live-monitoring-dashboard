using System;

namespace MediaDashboard.Operations.Api.Models
{
    public class ProgramUpdateSettings
    {
        public string Description { get; set; }

        public TimeSpan ArchiveWindowLength { get; set; }
    }
}
