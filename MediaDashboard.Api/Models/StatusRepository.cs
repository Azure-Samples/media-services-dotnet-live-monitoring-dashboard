using MediaDashboard.Common;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Persistence.Caching;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Api.Models
{
    public class StatusRepository : IStatusRepository
    {
        private IMdCache _cache = MdCache.Instance;

        public List<Data.AmsAccountStatus> GetAmsAccountStatus()
        {
            var accountIds = GetMediaAccountIds();

            var retval = accountIds
                .Denull()
                .Select(
                    accountId =>
                    {
                        var mediaService = _cache.GetAs<MediaService>(MediaService.GetCacheKey(accountId));
                        if (mediaService != null)
                            return Mappers.MediaServiceToAmsAccountStatus.Map(mediaService);
                        return null;
                    })
                .Where(x => null != x)
                .ToList();

            //List<Data.AmsAccountStatus> amsAccountStatusList = Mappers.MediaServiceToAmsAccountStatus.Map(config);

            //var retval = new List<Data.AmsAccountStatus>();

            //var amsAccountStatus = new Data.AmsAccountStatus();
            //amsAccountStatus.Id = Guid.NewGuid().ToString();
            
            //var status = new Data.Status()
            //{
            //    ChannelStatus = 1,
            //    ProgramStatus = 1,
            //    OriginStatus = 1
            //};
            //amsAccountStatus.Status = status;

            //retval.Add(amsAccountStatus);
            //retval.Add(amsAccountStatus);
            //retval.Add(amsAccountStatus);

            return retval;
        }

        public Data.AmsAccountStatus GetAmsAccountStatusById(string amsAccountId)
        {
            var mediaService = _cache.GetAs<MediaService>(MediaService.GetCacheKey(amsAccountId));

            var amsAccountStatus = Mappers.MediaServiceToAmsAccountStatus.Map(mediaService);

            //var amsAccountStatus = new Data.AmsAccountStatus();
            //amsAccountStatus.Id = Guid.NewGuid().ToString();

            //var status = new Data.Status()
            //{
            //    ChannelStatus = 2,
            //    ProgramStatus = 2,
            //    OriginStatus = 2
            //};
            //amsAccountStatus.Status = status;

            return amsAccountStatus;
        }

        public List<Data.PipelineStatus> GetPipelineStatus(string amsAccountId)
        {
            var mediaService = _cache.GetAs<MediaService>(MediaService.GetCacheKey(amsAccountId));

            var retval = new List<Data.PipelineStatus>();

            foreach (var mediaChannel in mediaService.Channels)
            {
                var pipelineStatus = Mappers.MediaServiceToPipelineStatus.Map(mediaService, mediaChannel);

                retval.Add(pipelineStatus);
            }

            //var retval = new List<Data.ChannelStatus>();
            //var channelStatus = new Data.ChannelStatus();
            //channelStatus.Id = Guid.NewGuid().ToString();

            //var status = new Data.Status()
            //{
            //    ChannelStatus = 3,
            //    ProgramStatus = 3,
            //    OriginStatus = 3
            //};
            //channelStatus.Status = status;

            //retval.Add(channelStatus);
            //retval.Add(channelStatus);
            //retval.Add(channelStatus);
            
            return retval;
        }

        public Data.PipelineStatus GetPipelineStatusById(string amsAccountId, string channelId)
        {
            var mediaService = _cache.GetAs<MediaService>(MediaService.GetCacheKey(amsAccountId));
            var mediaChannel = mediaService.Channels.Where(x => x.Id == channelId).FirstOrDefault();

            var pipelineStatus = Mappers.MediaServiceToPipelineStatus.Map(mediaService, mediaChannel);
            
            //var channelStatus = new Data.ChannelStatus();
            //channelStatus.Id = Guid.NewGuid().ToString();
            
            //var status = new Data.Status()
            //{
            //    ChannelStatus = 4,
            //    ProgramStatus = 4,
            //    OriginStatus = 4
            //};
            //channelStatus.Status = status;

            return pipelineStatus;
        }

        private List<string> GetMediaAccountIds()
        {
            var config = App.Config.Content;

            var list = new List<string>();
            foreach(var configContentProvider in config.ContentProviders)
            {
                foreach(var mediaServicesSet in configContentProvider.MediaServicesSets)
                {
                    foreach(var mediaServicesAccount in mediaServicesSet.MediaServicesAccounts)
                    {
                        list.Add(mediaServicesAccount.Id);
                    }
                }
            }

            return list;
        }
    }
}