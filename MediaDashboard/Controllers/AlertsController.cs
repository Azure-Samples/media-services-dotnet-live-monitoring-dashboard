using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Models;
using MediaDashboard.Persistence.Storage;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MediaDashboard.Controllers
{
    [Route("/api/accounts/{account}/[controller]")]
    public abstract class AlertsController : ControllerBase
    {
        protected bool ValidateQuery(AlertsQuery query)
        {
            if (query != null)
            {
                if (query.EndTime != default(DateTime))
                {
                    query.EndTime = query.EndTime.ToUniversalTime();
                }
                else
                {
                    query.EndTime = DateTime.UtcNow;
                }
                
                if (query.StartTime != default(DateTime))
                {
                    query.StartTime = query.StartTime.ToUniversalTime();
                }
                else 
                {
                    query.StartTime = query.EndTime.AddHours(-1);
                }

                if (query.EndTime <= query.StartTime)
                {
                    return false;
                }
            }
            return true;
        }

        protected List<T> GetAlertsFromCache<T>(string id)
        {
            string alertstring = CloudCache.Get(id + "Alerts"); 
            if (alertstring != null)
            {
                return  JsonConvert.DeserializeObject<List<T>>(alertstring);
            }
            return null;
        }
    }
}
