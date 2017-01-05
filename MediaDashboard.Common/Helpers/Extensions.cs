using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using Microsoft.WindowsAzure.MediaServices.Client;
using System.Linq;

namespace MediaDashboard.Common.Helpers
{
    public static class Extensions
    {
        public static MediaServicesSetConfig GetCustomer(this MediaDashboardConfig config, string accountSetName)
        {
            return config.Content.ContentProviders
                .SelectMany(contentProvider => contentProvider.MediaServicesSets)
                .FirstOrDefault(mediaServiceSet => mediaServiceSet.Name == accountSetName);
        }

        /**
        * Get the MediaServicesSet containing the given account name.
        */
        public static MediaServicesSetConfig GetMediaServicesSet(this MediaDashboardConfig config, string accountName)
        {
            return config.Content.ContentProviders
                .SelectMany(contentProvider => contentProvider.MediaServicesSets)
                .FirstOrDefault(mediaServicesSet => mediaServicesSet.MediaServicesAccounts.Any(account => account.AccountName == accountName));
        }

        public static MediaServicesAccountConfig GetMediaServicesAccount(this MediaDashboardConfig config, string accountName)
        {
            return config.Content.ContentProviders
                .SelectMany(contentProvider => contentProvider.MediaServicesSets)
                .SelectMany(mediaServicesSet => mediaServicesSet.MediaServicesAccounts)
                .FirstOrDefault(account => account.AccountName == accountName);
        }

        public static HealthStatus GetHealthStatus(this string level)
        {
            switch(level.ToLower())
            {
                case "normal":
                case "healthy":
                    return HealthStatus.Healthy;
                case "warning":
                    return HealthStatus.Warning;
                case "critical":
                    return HealthStatus.Critical;
                default:
                    return HealthStatus.Ignore;
            }
        }

        private static string GetUrl(string originHostName, string url)
        {
            string protocol = "https";
            if(originHostName.Contains(".origin."))
            {
                // use http for legacy origins.
                protocol = "http";
            }
            return string.Format("{0}://{1}{2}", protocol, originHostName, url);
        }

        public static void UpdateStreamingUrls(this ProgramUrls program, string origin)
        {
            // If the program asset has any locators.. update to include the correct origin hostname.
            if(program.SmoothStreamUrl != null)
            {
                program.SmoothStreamUrl = GetUrl(origin, program.SmoothStreamUrl);
                program.HLSUrl = GetUrl(origin, program.HLSUrl);
                program.HDSUrl = GetUrl(origin, program.HDSUrl);
                program.MPEGDashUrl = GetUrl(origin, program.MPEGDashUrl);
            }
        }


        public static void FindChannelOriginMapping(this MediaService account)
        {
            foreach(var channel in account.Channels)
            {
                channel.Programs = account.Programs
                    .Where(program => program.ChannelId.NimbusIdToRawGuid() == channel.Id)
                    .ToList();

                var origin = account.Origins.FindOrigin(channel.Name);
               
                channel.OriginId = origin?.Id;
                channel.OriginHostName = origin?.HostName;

                channel.Programs.ForEach(program =>
                {
                    // only if programs are running set the levels for programs.
                    if (program.State == ProgramState.Running.ToString())
                    {
                        program.ArchiveHealth = channel.ArchiveHealth;
                        program.OriginHealth = origin.Health;
                    }
                });

                // only if channel is running then show the origin level.
                if(channel.State == ChannelState.Running.ToString())
                {
                    channel.OriginHealth = origin.Health;
                }
                channel.Health = new[] { channel.IngestHealth, channel.EncodingHealth, channel.ArchiveHealth, channel.OriginHealth }.Max();
            }

            account.VodOrigins = account.Origins
                                    .Where(origin => !account.Channels.Any(channel => channel.Name == origin.Name))
                                    .Select(origin => origin.Id)
                                    .ToList();

            account.Health = account.Channels.Select(ch => ch.Health).Concat(account.Origins.Select(o => o.Health)).Max();
        }
    }
}
