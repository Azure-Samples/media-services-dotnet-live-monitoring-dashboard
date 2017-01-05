using MediaDashboard.Api.Models;
using MediaDashboard.Common.Data;
using System.Collections.Generic;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class MediaLocatorController : ApiController
    {
        private MediaLocatorRepo myRepo = new MediaLocatorRepo();
        public IEnumerable<MediaLocator> Get()
        {
            return (IEnumerable<MediaLocator>)myRepo.GetAll();
        }

        public MediaLocator Get(string id)
        {

            return (MediaLocator)myRepo.Get(id);

        }
    }
}
