using MediaDashboard.Operations.Api.Models;
using MediaDashboard.Persistence.Storage;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MediaDashboard.Operations.Api.Controllers
{
    public class DashboardAlertsController : ControllerBase
    {
        [Route("api/DashboardAlerts")]
        public IEnumerable<DashboardAlert> Get()
        {
            List<GlobalAlert> alerts = DataAccess.GetGlobalAlerts();//.GetMediaServicesAccounts().ToList();

            List < DashboardAlert >  daAlerts = new List<DashboardAlert>();
            if (alerts != null)
            {
                foreach (GlobalAlert alt in alerts.OrderByDescending(a => a.Timestamp))
                {
                    daAlerts.Add(new DashboardAlert
                    {
                        Name = alt.MediaServicesAccountName,
                        Date = alt.Timestamp,
                        Description = alt.Details,
                        Status = alt.ErrorLevel
                    });
                }
            }

            /* can't do it this way because the connection to the db is closed at this point
            //var list = alerts
            //    .OrderByDescending(o => o.Timestamp)
            //    .Select(s => new DashboardAlert()
            //    {
            //        Name = s.MediaServicesAccountName,
            //        Date = s.Timestamp,
            //        Description = s.Details,
            //        Status = s.ErrorLevel
            //    }).ToList();
            */
            return daAlerts;
        }
    }
}
