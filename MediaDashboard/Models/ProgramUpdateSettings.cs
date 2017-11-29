using System;

namespace MediaDashboard.Models
{
    public class ProgramUpdateSettings
    {
        public string Description { get; set; }

        public TimeSpan ArchiveWindowLength { get; set; }
    }
}
