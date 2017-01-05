using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Operations.Api.Models;
using MediaDashboard.Persistence.Storage;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Controllers
{
    public class ChannelAlertsController : ControllerBase
    {
        //accounts/nbcsgliveprodwestms/channelalerts/01fb2778-02c5-4247-9ac3-50169126fa47/originId     
        [Route("api/accounts/{account}/ChannelAlerts/{id}/{originId}")]
        public IEnumerable<MetricAlert> Get(string account, string id, [FromUri] AlertsQuery query, string originId = null)
        {
            var config = App.Config.GetMediaServicesSet(account);
            if(config == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var channelAlerts = Get(config, id, query);
            if (originId != null && (query.MetricTypes == null || query.MetricTypes.Contains(MetricType.Origin)))
            {
                query.MetricTypes = null;
                channelAlerts = channelAlerts.Concat(GetOriginAlerts(config, originId, query));
            }
            return channelAlerts.OrderByDescending(alert => alert.Date);
        }

        private IEnumerable<MetricAlert> Get(MediaServicesSetConfig  config, string channelId, AlertsQuery query)
        {
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddHours(-1);
            HealthStatus[] healthLevels = null;
            MetricType[] metricTypes = null;

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
                metricTypes = query.MetricTypes;
                healthLevels = query.StatusLevels;
            }

            var chid = channelId.GuidToChannelId();
            var alertstring = CloudCache.Get(chid + "Alerts");
            List<ChannelAlert> channelAlerts;
            if (alertstring != null)
            {
                channelAlerts = JsonConvert.DeserializeObject<List<ChannelAlert>>(alertstring);
            }
            else
            {
                var dataAccess = new AzureDataAccess(config.DataStorageConnections);
                channelAlerts = dataAccess.ExecuteReadQuery(context =>
                {
                    var dbAlerts = context.ChannelAlerts.Where( alert => 
                        alert.ChannelId == channelId &&
                        alert.Timestamp >= startTime &&
                        alert.Timestamp <= endTime);

                    if (healthLevels != null)
                    {
                        var predicate = CreatePredicate(healthLevels, "ErrorLevel");
                        dbAlerts = dbAlerts.Where(predicate);
                    }

                    if(metricTypes != null)
                    {
                        var condition = CreatePredicate(metricTypes, "MetricType");
                        dbAlerts = dbAlerts.Where(condition);
                    }
                    return dbAlerts.ToList();
                });
            }

            IEnumerable<MetricAlert> alerts = null;
            if (channelAlerts != null)
            {
                alerts = channelAlerts.Select(alert => new MetricAlert
                {
                    Name = alert.ChannelId,
                    Date = alert.Timestamp,
                    Description = alert.Details,
                    Status = alert.ErrorLevel
                });

            }
            return alerts;
        }

        private static Expression<Func<ChannelAlert, bool>> CreatePredicate<T>(IEnumerable<T> values, string property)
        {
            var parameter = Expression.Parameter(typeof(ChannelAlert), "alert");
            var left = Expression.Property(parameter, property);
            Expression expression = null;
            foreach (var value in values)
            {
                var right = Expression.Constant(value.ToString());
                var compare = Expression.Equal(left, right);
                expression = expression == null ? compare : Expression.Or(expression, compare);
            }
            var condition = Expression.Lambda<Func<ChannelAlert, bool>>(expression, parameter);
            return condition;
        }

        private IEnumerable<MetricAlert> GetOriginAlerts(MediaServicesSetConfig config, string originId, AlertsQuery query)
        {
            var controller = new OriginAlertsController();
            return controller.Get(config, originId, query);
        }
    }
}
