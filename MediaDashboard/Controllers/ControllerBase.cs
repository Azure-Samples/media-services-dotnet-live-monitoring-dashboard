using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Persistence.Caching;
using MediaDashboard.Persistence.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MediaDashboard.Controllers
{
  [Route("api/accounts/{account}/[controller]")]
  [Authorize]
    public abstract class ControllerBase : Controller
    {
        public IMdCache CloudCache => MdCache.Instance;

        public MediaServicesAccountConfig DefaultAccount => App.Config.GetDefaultAccount();

        public AzureDataAccess DataAccess => GetDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[0]);

        protected static AzureDataAccess GetDataAccess(MediaServicesSetConfig config)
        {
            return new AzureDataAccess(config.DataStorageConnections);
        }       
    }
}
