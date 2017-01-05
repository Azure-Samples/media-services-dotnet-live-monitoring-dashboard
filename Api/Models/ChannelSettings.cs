using MediaDashboard.Common.Helpers;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaDashboard.Operations.Api.Models
{
    public class ChannelSettings : ChannelUpdateSettings
    {
        public ChannelSettings()
        {
            EncodingType = ChannelEncodingType.Standard;
            StreamingProtocol = StreamingProtocol.RTPMPEG2TS;
            EncodingPreset = "Default720p";
        }

        public string Name { get; set; }

        public ChannelEncodingType EncodingType { get; set; }

        public StreamingProtocol StreamingProtocol { get; set; }

        public string EncodingPreset { get; set; }

        public string SlateAssetId { get; set; }


        public ChannelCreationOptions GetChannelCreationOptions()
        {
            return new ChannelCreationOptions
            {
                Name = Name,
                Description = Description,
                EncodingType = EncodingType,
                Input = ChannelCreationOperations.ConfigureDefaultInput(StreamingProtocol, IPRange.ToSdk(IngestAllowList)),
                Preview = ChannelCreationOperations.ConfigureChannelPreview(IPRange.ToSdk(PreviewAllowList)),
                Output = new ChannelOutput(),
                Encoding = EncodingType != ChannelEncodingType.None ? ChannelCreationOperations.GetDefaultEncoding(EncodingPreset) : null,
                Slate = EncodingType != ChannelEncodingType.None && SlateAssetId != null ? ChannelCreationOperations.GetDefaultSlate(SlateAssetId) : null
            };
        }
    }
}
