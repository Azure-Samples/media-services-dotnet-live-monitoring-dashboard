using System.Linq;
using MediaDashboard.Api.Data;
using System.Collections.Generic;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Persistence.Caching;

namespace MediaDashboard.Api.Mappers
{
    public static class ContentConfigToInformation
    {
        public static Information Map(ContentConfig contentConfig)
        {
            var info = new Information();
            info.DashboardTitle = contentConfig.DashboardTitle;
            info.ContentProviders = new List<AContentProvider>();
            
            info.ContentProviders = contentConfig.ContentProviders
                .Denull()
                .Select(MapContentProvider)
                .ToList();

            return info;
        }
        
        private static AContentProvider MapContentProvider(ContentProviderConfig configContentProvider)
        {
            var contentProvider = new AContentProvider();
            contentProvider.Id = configContentProvider.Id;
            contentProvider.Name = configContentProvider.Name;
            contentProvider.MediaPipelines = configContentProvider.MediaServicesSets
                .Denull()
                .Select(MapMediaServicesSet)
                .ToList();

            return contentProvider;
        }

        private static MediaPipeline MapMediaServicesSet(MediaServicesSetConfig configMediaServiceSet)
        {
            var mediaPipeline = new MediaPipeline();
            mediaPipeline.Name = configMediaServiceSet.Name;
            mediaPipeline.Deployments = configMediaServiceSet.MediaServicesAccounts
                .Denull()
                .Select(MapMediaServicesAccounts)
                .Where(ad => null != ad)
                .ToList();

            return mediaPipeline;
        }

        private static ADeployment MapMediaServicesAccounts(MediaServicesAccountConfig configMediaServicesAccount)
        {
            var mediaService = MdCache.Instance.GetAs<MediaService>(
                MediaService.GetCacheKey(configMediaServicesAccount.Id)
                );

            if (null == mediaService)
                return null;

            var deployment = new ADeployment();
            deployment.Id = configMediaServicesAccount.Id;
            deployment.AccountName = configMediaServicesAccount.AccountName;
            deployment.Location = configMediaServicesAccount.MetaData.Location;

            deployment.Channels = mediaService.Channels
                .Denull()
                .Select(mch => MapMediaChannel(configMediaServicesAccount, mediaService, mch))
                .ToList();

            return deployment;
        }

        private static AChannel MapMediaChannel(
            MediaServicesAccountConfig configMediaServicesAccount, 
            MediaService mediaService, 
            MediaChannel mediaChannel
            )
        {
            var achannel = new AChannel();

            string originId = configMediaServicesAccount.OriginMappings
                .Denull()
                .Where(x => x.ChannelId == mediaChannel.Id)
                .Select(x => x.OriginId)
                .FirstOrDefault();

            achannel.Origin = new AOrigin { Id = originId };

            achannel.Programs = mediaService.Programs
                .Denull()
                .Where(mp => mp.ChannelId == mediaChannel.Id)
                .Select(mp =>
                    new AProgram()
                    {
                        Asset = new AAsset()
                    }
                )
                .ToList();


            return achannel;
        }
    }
}