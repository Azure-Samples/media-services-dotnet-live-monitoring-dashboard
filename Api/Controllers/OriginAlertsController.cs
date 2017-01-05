using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Operations.Api.Models;
using MediaDashboard.Persistence.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Controllers
{
    public class OriginAlertsController : ControllerBase
    {
        //accounts/nbcsgliveprodwestms/channelalerts/01fb2778-02c5-4247-9ac3-50169126fa47     
        public IEnumerable<MetricAlert> Get(string account, string id, [FromUri] AlertsQuery query)
        {
            var config = App.Config.GetMediaServicesSet(account);
            if (config == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            return Get(config, id, query);
        }

        internal IEnumerable<MetricAlert> Get(MediaServicesSetConfig config, string originId, AlertsQuery query)
        {
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddHours(-1);
            HealthStatus[] healthLevels = null;

            if (query != null)
            {
                if (query.StartTime != default(DateTime))
                {
                    startTime = query.StartTime.ToUniversalTime();
                }
                if (query.EndTime != default(DateTime))
                {
                    endTime = query.EndTime.ToUniversalTime();
                }
                if (endTime <= startTime)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
                }
                healthLevels = query.StatusLevels;
            }

            var origId = originId.GuidToOriginId();
            var alertstring = CloudCache.Get(origId + "Alerts");
            List<OriginAlert> alerts;
            if (alertstring != null)
            {
                alerts = JsonConvert.DeserializeObject<List<OriginAlert>>(alertstring);
            }
            else
            {
                var dataAccess = new AzureDataAccess(config.DataStorageConnections);
                alerts = dataAccess.ExecuteReadQuery(context =>
                {
                    var dbAlerts = context.OriginAlerts.Where(alert =>
                       alert.OriginId == originId &&
                       alert.Timestamp >= startTime &&
                       alert.Timestamp <= endTime);

                    if (healthLevels != null)
                    {
                        var predicate = CreatePredicate(healthLevels, "ErrorLevel");
                        dbAlerts = dbAlerts.Where(predicate);
                    }
                    return dbAlerts.ToList();
                });
            }

            return alerts.Select(alert => new MetricAlert
            {
                Name = alert.OriginId,
                Date = alert.Timestamp,
                Description = alert.Details,
                Status = alert.ErrorLevel
            }).OrderByDescending(alt => alt.Date);
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