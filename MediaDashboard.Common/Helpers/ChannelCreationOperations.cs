using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

using Microsoft.WindowsAzure.MediaServices.Client;
using System.Net;
using System.Linq;

namespace MediaDashboard.Common.Helpers
{
    public class ChannelCreationOperations
    {
        public static X509Certificate2 GetCertificate(string certThumbprint)
        {
            // Create a reference to the My certificate store.
            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Try to open the store.
            try
            {
                certStore.Open(OpenFlags.ReadOnly);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error reading certificate with thumbprint {0}! {1}", certThumbprint, e);
                throw;
            }

            // Find the certificate that matches the thumbprint.
            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);
            certStore.Close();

            // Check to see if our certificate was added to the collection. If no, throw an error, if yes, create a certificate using it.
            if (0 == certCollection.Count)
            {
                throw new Exception("Error: No certificate found containing thumbprint " + certThumbprint);
            }

            var certificate = certCollection[0];

            if (!certificate.HasPrivateKey)
            {
                throw new Exception("No private key found for certificate");
            }

            return certificate;
        }

        public static HttpClient GetWebClient(Uri ApiServer, X509Certificate2 certificate)
        {
            var webRequestHandler = new WebRequestHandler();


            if (certificate != null)
            {
                webRequestHandler.ClientCertificates.Add(certificate);
            }
            var client = new HttpClient(webRequestHandler);
            client.BaseAddress = ApiServer;

            return client;
        }

        public static ChannelCreationOptions CreateNewChannelOptions(
            string name,
            ChannelEncodingType encodingType,
            StreamingProtocol protocol,
            string encodingPreset,
            string slateAssetId
            )
        {
            return new ChannelCreationOptions
            {
                Name = string.Format("New-Channel-{0}", DateTime.UtcNow.ToOADate().ToString().Replace(".", "-")),
                Description = "Newly Created Channel",
                EncodingType = encodingType,
                Input = ConfigureDefaultInput(protocol, GetDefaultIpAllowList()),
                Preview = ConfigureChannelPreview(null),
                Output = new ChannelOutput(),
                Encoding = encodingType != ChannelEncodingType.None ? GetDefaultEncoding(encodingPreset) : null,
                Slate = encodingType != ChannelEncodingType.None ? GetDefaultSlate(slateAssetId) : null
            };
        }

        public static ChannelPreview ConfigureChannelPreview(List<IPRange> allowList)
        {
            return new ChannelPreview
            {
                AccessControl = allowList.CreateChannelAccessControl()
            };
        }

        public static ChannelInput ConfigureDefaultInput(StreamingProtocol protocol, List<IPRange> allowList)
        {
            return new ChannelInput
            {
                StreamingProtocol = protocol,
                AccessControl = allowList.CreateChannelAccessControl()
            };
        }

        public static List<IPRange> GetIpAllowList(string allowList)
        {
            return allowList.Split(';').Select( (ip, i) => {
                var parts = ip.Split('/');
                return new IPRange
                {
                    Name = "Range" + i,
                    Address = IPAddress.Parse(parts[0]),
                    SubnetPrefixLength = int.Parse(parts[1])
                };
            }).ToList();
        }

        public static List<IPRange> GetDefaultIpAllowList()
        {
            return new List<IPRange> { GetDefaultIPRange() };
        }

        public static ChannelEncoding GetDefaultEncoding(string preset)
        {
            return new ChannelEncoding
            {
                SystemPreset = preset
            };
        }

        public static ChannelSlate GetDefaultSlate(string slateAssetId)
        {
            return new ChannelSlate
            {
                InsertSlateOnAdMarker = true,
                DefaultSlateAssetId = slateAssetId
            };
        }

        public static IPRange GetDefaultIPRange()
        {
            return new IPRange
            {
                Name = "Allow All",
                Address = IPAddress.Parse("0.0.0.0"),
                SubnetPrefixLength = 0
            };
        }
    }
}
