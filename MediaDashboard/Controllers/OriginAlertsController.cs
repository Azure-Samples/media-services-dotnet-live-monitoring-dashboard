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
    public class OriginAlertsController : AlertsController
    {
        //accounts/nbcsgliveprodwestms/channelalerts/01fb2778-02c5-4247-9ac3-50169126fa47
        [HttpGet("{id}")]
        public IActionResult Get(string account, string id, [FromQuery] AlertsQuery query)
        {
            var config = App.Config.GetMediaServicesSet(account);
            if (config == null)
            {
                return NotFound();
            }

            if (!ValidateQuery(query))
            {
                return BadRequest();
            }

            return Ok(Get(config, id, query));
        }

        internal List<MetricAlert> Get(MediaServicesSetConfig config, string originId, AlertsQuery query)
        {
            //var origId = originId.GuidToOriginId();
            var originAlerts = GetAlertsFromCache<OriginAlert>(originId);
            if (originAlerts == null)
            {
                var dataAccess = new AzureDataAccess(config.DataStorageConnections);
                if (query.StatusLevels == null)
                {
                    originAlerts = dataAccess.GetOriginAlerts(originId);
                }
                else
                {
                    originAlerts = dataAccess.GetOriginAlerts(originId, query.StatusLevels);
                }
            }

            List<MetricAlert> alerts = new List<MetricAlert>();
            if (originAlerts != null && originAlerts.Count > 0)
            {
                foreach (var orgAlt in originAlerts)
                {
                    alerts.Add(new MetricAlert
                    {
                        AlertID = orgAlt.AlertID,
                        Name = orgAlt.OriginName,
                        Date = orgAlt.AlertDate.AddTicks(orgAlt.AlertTime.Ticks),
                        Description = orgAlt.Details,
                        Status = orgAlt.ErrorLevel
                    });
                }
            }

            return alerts;
        }

        private static Expression<Func<OriginAlert, bool>> CreatePredicate<T>(IEnumerable<T> values, string property)
        {
            var parameter = Expression.Parameter(typeof(OriginAlert), "alert");
            var left = Expression.Property(parameter, property);
            Expression expression = null;
            foreach (var value in values)
            {
                var right = Expression.Constant(value.ToString());
                var compare = Expression.Equal(left, right);
                expression = expression == null ? compare : Expression.Or(expression, compare);
            }
            var condition = Expression.Lambda<Func<OriginAlert, bool>>(expression, parameter);
            return condition;
        }
    }
}
