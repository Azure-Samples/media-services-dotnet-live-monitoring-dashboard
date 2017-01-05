using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaDashboard.Common.Helpers
{
    public static class MediaServiceExtensions
    {
        public static CloudMediaContext GetContext(this MediaServicesAccountConfig accountConfig)
        {
            var credentials = new MediaServicesCredentials(accountConfig.AccountName, accountConfig.AccountKey);
            if (accountConfig.MetaData.AcsScope != null)
            {
                credentials.Scope = accountConfig.MetaData.AcsScope;
                credentials.AcsBaseAddress = accountConfig.MetaData.AcsBaseAddress;
            }

            var cloudContext = accountConfig.AdminUri != null ?
                new CloudMediaContext(accountConfig.AdminUri, credentials) :
                new CloudMediaContext(credentials);

            return cloudContext;
        }

        public static string NimbusIdToRawGuid(this string nimbusId)
        {
            string formattedId = string.Empty;
            string[] split = nimbusId.Split(':');
            if (split.Length > 1)
                formattedId = split[3];
            else
                formattedId = nimbusId;

            return formattedId;
        }

        public static Guid NimbusIdToGuid(this string nimbusId)
        {
            return new Guid(NimbusIdToRawGuid(nimbusId));
        }


        // simple helper method to pick the first media acocunt of the first customer.
        public static MediaServicesAccountConfig GetDefaultAccount(this MediaDashboardConfig config)
        {
            return config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0];
        }

        public static string GuidToChannelId(this string guid)
        {
            return string.Format("nb:chid:UUID:{0}", guid);
        }

        public static string GuidToOriginId(this string guid)
        {
            return string.Format("nb:oid:UUID:{0}", guid);
        }

        public static string GuidToProgramId(this string guid)
        {
            return string.Format("nb:pgid:UUID:{0}", guid);
        }

        public static string GetRunningTime(this DateTime startTime)
        {
            var runningTime = DateTime.UtcNow.Subtract(startTime);
            return string.Format("{0}d {1}:{2}:{3}",
                runningTime.Days,
                runningTime.Hours,
                runningTime.Minutes,
                runningTime.Seconds);
        }

        public static string GetUpdatedUri(this Uri baseUri,
            IStreamingEndpoint origin,
            string pathSuffix = null)
        {
            var uriBuilder = new UriBuilder(baseUri);
            if (origin != null)
            {
                uriBuilder.Host = origin.HostName;
                // work around for old origins.
                uriBuilder.Scheme = origin.HostName.Contains(".origin.") ? Uri.UriSchemeHttp : Uri.UriSchemeHttps;
                uriBuilder.Port = -1;
            }
            if (pathSuffix != null)
            {
                uriBuilder.Path += pathSuffix;
            }
            return uriBuilder.Uri.AbsoluteUri;
        }

        // Match by name or pick the first one.
        public static MediaOrigin FindOrigin(this List<MediaOrigin> origins, string channelName)
        {
            var origin = origins.FirstOrDefault(o => o.Name == channelName);
            if (origin == null)
            {
                origin = origins.FirstOrDefault();
            }
            return origin;
        }


        public static string GetThumbnailUrl(this IChannel channel)
        {
            string thumbnailUrl = null;
            if (channel.State == ChannelState.Running && channel.EncodingType != ChannelEncodingType.None)
            {
                var preview = channel.Preview.Endpoints[0].Url;
                var uri = new UriBuilder
                {
                    Scheme = Uri.UriSchemeHttps,
                    Host = preview.Host,
                    Path = "thumbnails/input.jpg"
                }.Uri;
                thumbnailUrl = uri.AbsoluteUri;
            }
            return thumbnailUrl;
        }


        public static string GetRunningTime(this IChannel channel)
        {
            string runTime = string.Empty;
            if (channel.State == ChannelState.Running)
            {
                runTime = channel.LastModified.GetRunningTime();
            }
            return runTime;
        }

        public static ProgramUrls GetStreamingUrls(this IProgram program)
        {
            var locators = program.Asset.Locators.ToList();
            var locator = locators.FirstOrDefault(l => l.Type == LocatorType.OnDemandOrigin);
            if (locator != null)
            {
                var baseUri = locator.GetSmoothStreamingUri();
                return new ProgramUrls
                {
                    SmoothStreamUrl = GetUrl(baseUri.AbsolutePath, string.Empty),
                    HLSUrl = GetUrl(baseUri.AbsolutePath, "(format=m3u8-aapl)"),
                    HDSUrl = GetUrl(baseUri.AbsolutePath, "(format=f4m-f4f).f4m"),   //the HDS Extension is not in the ILocatorExtensions class
                    MPEGDashUrl = GetUrl(baseUri.AbsolutePath, "(format=mpd-time-csf)")
                };
            }
            return null;
        }

        private static string GetUrl(string absolutePath, string format)
        {
            return string.Format("{0}{1}", absolutePath, format);
        }

        public static string GetAllowList(this IList<IPRange> iPAllowList)
        {
            return string.Join(
                ";",
                iPAllowList.Select(ipRange => string.Format("{0}/{1}", ipRange.Address, ipRange.SubnetPrefixLength)));
        }


        public static Task<List<MediaChannel>> GetChannels(this CloudMediaContext context)
        {
            return Task.Run(() =>
            {
                return context.Channels.Select(EntityFactory.BuildChannelFromIChannel).ToList();
            });
        }

        public static ChannelAccessControl CreateChannelAccessControl(this IList<IPRange> allowList)
        {
            return allowList == null ? null : new ChannelAccessControl
            {
                IPAllowList = allowList
            };
        }
    }
}
