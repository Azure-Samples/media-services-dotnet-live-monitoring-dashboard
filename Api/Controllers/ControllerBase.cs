using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Persistence.Caching;
using MediaDashboard.Persistence.Storage;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MediaDashboard.Operations.Api.Controllers
{
    [Authorize]
    public abstract class ControllerBase : Controller
    {

        public IMdCache CloudCache { get; private set; }

        public AzureDataAccess DataAccess { get; private set; }

        // TODO: temporary - removed once all controllers take the acocunt name/id as argument.
        public MediaServicesAccountConfig DefaultAccount { get; private set; }

        public ControllerBase()
        {
            CloudCache = MdCache.Instance;
            DataAccess = GetDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[0]);
            DefaultAccount = App.Config.GetDefaultAccount();
        }

        public static AzureDataAccess GetDataAccess(MediaServicesSetConfig config)
        {
            return new AzureDataAccess(config.DataStorageConnections);
        }
       
        protected virtual HealthStatus GetMaxLevel(IEnumerable<GlobalAlert> alerts)
        {
            if (alerts != null)
            {
                if (alerts.OrderByDescending(alt => alt.Timestamp).FirstOrDefault(f => f.ErrorLevel.Equals("Critical", System.StringComparison.InvariantCultureIgnoreCase)) != null)
                    return HealthStatus.Critical;
                else if (alerts.OrderByDescending(alt => alt.Timestamp).FirstOrDefault(f => f.ErrorLevel.Equals("Warning", System.StringComparison.InvariantCultureIgnoreCase)) != null)
                    return HealthStatus.Warning;
            }

            return HealthStatus.Healthy;
        }
    }
}