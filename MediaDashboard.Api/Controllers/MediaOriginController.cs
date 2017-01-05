using MediaDashboard.Api.Models;
using MediaDashboard.Common.Data;
using System.Collections.Generic;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class MediaOriginController : ApiController
    {
        private MediaOriginRepo myRepo = new MediaOriginRepo();
        public IEnumerable<MediaOrigin> Get()
        {
            return (IEnumerable<MediaOrigin>)myRepo.GetAll();
        }

        public MediaOrigin Get(string id)
        {

            return (MediaOrigin)myRepo.Get(id);

        }
    }
}
