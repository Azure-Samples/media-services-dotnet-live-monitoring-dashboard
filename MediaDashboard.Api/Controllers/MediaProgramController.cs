using MediaDashboard.Api.Models;
using MediaDashboard.Common.Data;
using System.Collections.Generic;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class MediaProgramController : ApiController
    {
        private MediaProgramRepo myRepo = new MediaProgramRepo();
        public IEnumerable<MediaProgram> Get()
        {
            return (IEnumerable<MediaProgram>)myRepo.GetAll();
        }

        public MediaProgram Get(string id)
        {

            return (MediaProgram)myRepo.Get(id);

        }
    }
}
