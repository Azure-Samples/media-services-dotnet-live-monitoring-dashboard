using MediaDashboard.Api.Models;
using MediaDashboard.Common.Data;
using System.Collections.Generic;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class MetricController : ApiController
    {
        private AmsAccountMetricRepo myRepo;
        [Route("api/Metric/AmsAccount/{idAmsAccount}")]
        public AmsAccountMetricRepo GetAmsMetric(string idAmsAccount)
        {
            myRepo = new AmsAccountMetricRepo(idAmsAccount);
            return myRepo;
        }
        [Route("api/Metric/AmsAccount/{idAmsAccount}/Channel")]
        public IEnumerable<MediaChannel> GetMetricChannel(string idAmsAccount)
        {
            myRepo = new AmsAccountMetricRepo(idAmsAccount);
            return myRepo.Channels;
        }
        [Route("api/Metric/AmsAccount/{idAmsAccount}/Channel/{idChannel}")]
        public MediaChannel GetAllMetricChannel(string idAmsAccount, string idChannel)
        {
            myRepo = new AmsAccountMetricRepo(idAmsAccount);
            return myRepo.GetChannelMetricDatail(idChannel);
        }


        // //[Route("api/Metric/AmsAccount/Channel")]
        // //public IEnumerable<string> GetAllMetricChannel()
        // //{
        // //    return new string[2] { "sasas", "sasasasasa" };
        // //}

        //[Route("api/Metric/AmsAccount/Channel/{channelId}")]
        //public IEnumerable<string> GetAllMetricChannel(string channelId)
        //{
        //    return null;
        //}

        

        
    }
}
