using MediaDashboard.Api.Data;
using MediaDashboard.Api.Models;
using System.Web.Http;

namespace MediaDashboard.Api.Controllers
{
    public class InformationController : ApiController
    {
        private readonly InformationRepo _repo;

        public InformationController()
        {
            _repo = new InformationRepo();
        }
        

        public Information Get()
        {
            return _repo.GetAs<Information>();
        }
    }
}