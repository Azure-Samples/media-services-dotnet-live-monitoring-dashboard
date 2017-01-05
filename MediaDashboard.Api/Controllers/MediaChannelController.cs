using MediaDashboard.Api.Models;
using MediaDashboard.Common.Data;
using System.Collections.Generic;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class MediaChannelController : ApiController
    {
        private MediaChannelsRepo myRepo = new MediaChannelsRepo();
        public IEnumerable<MediaChannel> Get()
        {
            return (IEnumerable<MediaChannel>)myRepo.GetAll();
        }

        public MediaChannel Get(string id)
        {
            
            return (MediaChannel)myRepo.Get(id);

        }

    }

    
}
