using MediaDashboard.Api.Models;
using MediaDashboard.Common.Data;
using System.Collections.Generic;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class MediaAssetController : ApiController
    {
        private MediaAssetRepo myRepo = new MediaAssetRepo();
        public IEnumerable<MediaAsset> Get()
        {
            return (IEnumerable<MediaAsset>)myRepo.GetAll();
        }

        public MediaAsset Get(string id)
        {

            return (MediaAsset)myRepo.Get(id);

        }
    }
}
