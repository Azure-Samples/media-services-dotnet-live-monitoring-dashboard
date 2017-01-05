using MediaDashboard.Api.Data;
using MediaDashboard.Api.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class StatusController : ApiController
    {
        private StatusRepository myRepo = new StatusRepository(); 

        [Route("api/Status/AmsAccount")]
        public List<AmsAccountStatus> Get()
        {

            return (List<AmsAccountStatus>)myRepo.GetAmsAccountStatus();

        }

        [Route("api/Status/AmsAccount/{amsAccountId}")]
        public AmsAccountStatus Get(string amsAccountId)
        {

            return (AmsAccountStatus)myRepo.GetAmsAccountStatusById(amsAccountId);

        }

        [Route("api/Status/AmsAccount/{amsAccountId}/Pipeline")]
        public List<PipelineStatus> GetChannel(string amsAccountId)
        {

            return (List<PipelineStatus>)myRepo.GetPipelineStatus(amsAccountId);

        }

        [Route("api/Status/AmsAccount/{amsAccountId}/Pipeline/{channelId}")]
        public PipelineStatus GetChannel(string amsAccountId, string channelId)
        {

            return (PipelineStatus)myRepo.GetPipelineStatusById(amsAccountId, channelId);

        }
    }
}