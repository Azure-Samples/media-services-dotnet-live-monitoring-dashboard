using MediaDashboard.Api.Data;
using MediaDashboard.Common.Data;
using System.Linq;

namespace MediaDashboard.Api.Mappers
{
    public static class MediaServiceToPipelineStatus
    {
        public static PipelineStatus Map(MediaService mediaService, MediaChannel mediaChannel)
        {
            var program = mediaService.Programs.Where(x => x.ChannelId == mediaChannel.Id).FirstOrDefault();

            var pipelineStatus = new Data.PipelineStatus();
            pipelineStatus.Id = mediaChannel.Id;

            var status = new Data.Status();
            status.ChannelStatus = (int)mediaChannel.Health;
            status.ProgramStatus = (int)program.Health;
            status.OriginStatus = -1; //TODO: GET THIS VIA THE NEW CONFIG SETTING;

            pipelineStatus.Status = status;

            return pipelineStatus;
        }
    }
}