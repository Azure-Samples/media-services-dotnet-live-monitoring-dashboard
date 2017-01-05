using MediaDashboard.Web.Api.Models;
using MediaDashboard.Persistence.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MediaDashboard.Web.Api.Controllers
{
    public class DashboardAlertsController : ControllerBase
    {
        [Route("api/DashboardAlerts")]
        public IEnumerable<DashboardAlert> Get()
        {
            List<GlobalAlert> alerts = DataAccess.GetAccountGlobalAlerts(DefaultAccount.AccountName);

            List < DashboardAlert >  daAlerts = new List<DashboardAlert>();
            if (alerts != null)
            {
                foreach (GlobalAlert alt in alerts)
                {
                    daAlerts.Add(new DashboardAlert
                    {
                        Name = alt.AccountName,
                        Date = alt.AlertDate.AddTicks(alt.AlertTime.Ticks),
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
