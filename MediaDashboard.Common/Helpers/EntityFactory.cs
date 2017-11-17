using MediaDashboard.Common.Data;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MediaDashboard.Common.Helpers
{

    public class EntityFactory
    {
        static public MediaChannel BuildChannelFromIChannel(IChannel ch)
        {
            var channel = new MediaChannel
            {
                Id = ch.Id.NimbusIdToRawGuid(),
                Name = ch.Name,
                Description = ch.Description,
                NameShort = ch.Name,
                IngestUrls = ch.Input.Endpoints.Select(e => e.Url.AbsoluteUri).ToArray(),
                PreviewUrl = ch.Preview.Endpoints[0].Url.AbsoluteUri,
                ThumbnailUrl = ch.GetThumbnailUrl(),
                State = ch.State.ToString(),
                EncodingType = ch.EncodingType.ToString(),
                EncodingPreset = ch.Encoding?.SystemPreset,
                FragmentDuration = ch.Input.KeyFrameInterval?.Seconds,
                HLSPackingRatio = ch.Output?.Hls?.FragmentsPerSegment,
                RunningTime = ch.GetRunningTime(),
                Created = ch.Created,
                LastModified = ch.LastModified,
                DefaultSlate = ch.Slate?.DefaultSlateAssetId,
                IngestAllowList = ch.Input.AccessControl?.IPAllowList.GetAllowList(),
                PreviewAllowList = ch.Preview.AccessControl?.IPAllowList.GetAllowList()
            };
            channel.ClientFragDiff = (channel.FragmentDuration != App.Config.Parameters.FragmentConfig.Duration);
            channel.ClientPackingRatioDiff = (channel.HLSPackingRatio.HasValue? (channel.HLSPackingRatio.Value != App.Config.Parameters.HLSConfig.HLSRatio): false);
            channel.ClientEncodingDiff = (!string.IsNullOrEmpty(channel.EncodingPreset) ? !string.Equals(channel.EncodingPreset, App.Config.Parameters.EncodingConfig.EncodingPresetName) : false);
            return channel;
        }


        static public MediaProgram BuildProgramFromIProgram(IProgram program)
        {
            return new MediaProgram()
            {
                ChannelId = program.ChannelId,
                Name = program.Name,
                Description = program.Description,
                Id = program.Id.NimbusIdToRawGuid(),
                Created = program.Created,
                LastModified = program.LastModified,
                State = program.State.ToString(),
                AssetId = program.AssetId,
                ManifestName = program.ManifestName,
                ArchiveWindowLength = program.ArchiveWindowLength,
                Health = (program.State == ProgramState.Running ? HealthStatus.Healthy : HealthStatus.Ignore)
            };
        }

        public static MediaOrigin BuildOriginFromIStreamingEndpoint(IStreamingEndpoint endpoint)
        {
            var origin = new MediaOrigin
            {
                Id = endpoint.Id.NimbusIdToRawGuid(),
                Name = endpoint.Name,
                NameShort = endpoint.Name.Substring(0, Math.Min(6, endpoint.Name.Length)).ToUpper(),
                Description = endpoint.Description,
                HostName = endpoint.HostName,
                LastModified = endpoint.LastModified,
                Created = endpoint.Created,
                State = endpoint.State.ToString(),
                ReservedUnits=(endpoint.ScaleUnits.HasValue ? endpoint.ScaleUnits.Value : 0),
                Health = endpoint.State == StreamingEndpointState.Running ? HealthStatus.Healthy : HealthStatus.Ignore,
                MaxAge = endpoint.CacheControl?.MaxAge
            };
            if (endpoint.AccessControl != null)
            {
                if (endpoint.AccessControl.IPAllowList != null)
                {
                    origin.IPAllowList = string.Join(";",
                        endpoint.AccessControl.IPAllowList
                        .Select(iprange => string.Format("{0}/{1}", iprange.Address, iprange.SubnetPrefixLength)));
                }
                if (endpoint.AccessControl.AkamaiSignatureHeaderAuthenticationKeyList != null)
                {
                    origin.AuthenticationKeys = endpoint.AccessControl?.AkamaiSignatureHeaderAuthenticationKeyList?.ToList();
                }
            }
            return origin;
        }

        public static List<MediaOrigin> BuildOriginListFromStreamingEndpoints(StreamingEndpointBaseCollection endpoints)
        {
            List<MediaOrigin> origins = new List<MediaOrigin>();
            foreach (IStreamingEndpoint ep in endpoints)
            {
                origins.Add(BuildOriginFromIStreamingEndpoint(ep));
            }
            return origins;
        }
    }
}
