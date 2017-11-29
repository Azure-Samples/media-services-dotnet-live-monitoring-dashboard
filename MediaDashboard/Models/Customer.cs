using System.Collections.Generic;
using MediaDashboard.Common.Data;

namespace MediaDashboard.Models
{
  public class Customer
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public int ChannelCount { get; set; }

    public int ArchiveCount { get; set; }

    public int OriginCount { get; set; }

    public int ProgramCount { get; set; }

    public List<MediaService> Accounts { get; set; }

    public HealthStatus Health { get; set; }
  }
}
