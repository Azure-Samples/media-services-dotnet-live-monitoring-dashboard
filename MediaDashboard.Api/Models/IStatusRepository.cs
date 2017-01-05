using MediaDashboard.Api.Data;
using System.Collections.Generic;

namespace MediaDashboard.Api.Models
{
    public interface IStatusRepository
    {
        List<AmsAccountStatus> GetAmsAccountStatus();

        AmsAccountStatus GetAmsAccountStatusById(string amsAccountId);

        List<PipelineStatus> GetPipelineStatus(string amsAccountId);

        PipelineStatus GetPipelineStatusById(string amsAccountId, string channelId);

    }
}