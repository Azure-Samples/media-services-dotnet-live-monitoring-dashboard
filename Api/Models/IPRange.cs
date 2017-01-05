using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sdk = Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaDashboard.Operations.Api.Models
{
    public class IPRange
    {
        public string Address { get; set; }

        public int SubnetPrefixLength { get; set; }

        public Sdk.IPRange ToSdk(string name)
        {
            return new Sdk.IPRange
            {
                Name = name,
                Address = IPAddress.Parse(Address),
                SubnetPrefixLength = SubnetPrefixLength
            };
        }

        public static List<Sdk.IPRange> ToSdk(IPRange[] ranges)
        {
            return ranges == null ?  null : ranges.Select((range, index) => range.ToSdk("Range" + index)).ToList();
        }
    }
}
