using MediaDashboard.Common.Helpers;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;

namespace MediaDashboard.Web.Api.Models
{
    public class OriginSettings: OriginUpdateSettings
    {
        public OriginSettings()
        {
            ScaleUnits = 1;
        }

        public string Name { get; set; }

        public int ScaleUnits { get; set; }

        public int MaxAge { get; set; }

        public StreamingEndpointCreationOptions GetCreationOptions()
        {
            return new StreamingEndpointCreationOptions
            {
                Name = Name,
                Description = Description,
                ScaleUnits = ScaleUnits,
                CacheControl = new StreamingEndpointCacheControl
                {
                    MaxAge = TimeSpan.FromMinutes(MaxAge)
                },
                AccessControl  = new StreamingEndpointAccessControl
                {
                    IPAllowList = AllowList == null ? ChannelCreationOperations.GetDefaultIpAllowList() : IPRange.ToSdk(AllowList)
                }
            };
        }
    }
}
