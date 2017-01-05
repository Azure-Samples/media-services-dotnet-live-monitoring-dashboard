using Microsoft.WindowsAzure.MediaServices.Client;
using System;

namespace MediaDashboard.Web.Api.Models
{
    public class ProgramSettings : ProgramUpdateSettings
    {
        public ProgramSettings ()
        {
            ArchiveWindowLength = TimeSpan.FromHours(4);
        }

        public string Name { get; set; }

        public string AssetName
        {
            get
            {
                return "Asset-" + Name;
            }
        }

        public string AssetId { get; set; }

        public string ManifestName { get; set; }

        public string LocatorId { get; set; }

        public ProgramCreationOptions GetCreationOptions()
        {
            return new ProgramCreationOptions
            {
                Name = Name,
                Description = Description,
                ManifestName = ManifestName,
                ArchiveWindowLength = ArchiveWindowLength
            };
        }
    }
}
