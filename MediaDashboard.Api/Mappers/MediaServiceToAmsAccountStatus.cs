using MediaDashboard.Api.Data;
using MediaDashboard.Common.Data;

namespace MediaDashboard.Api.Mappers
{
    public static class MediaServiceToAmsAccountStatus
    {
        public static AmsAccountStatus Map(MediaService mediaService)
        {
            var amsAccountStatus = new AmsAccountStatus();
            amsAccountStatus.Id = mediaService.Id;
            amsAccountStatus.Status = (int)mediaService.Health;

            return amsAccountStatus;
        }
    }
}