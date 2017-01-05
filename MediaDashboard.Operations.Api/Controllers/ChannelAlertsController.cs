using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Web.Api.Models;
using MediaDashboard.Persistence.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;

namespace MediaDashboard.Web.Api.Controllers
{
    public class ChannelAlertsController : ControllerBase
    {
        //accounts/nbcsgliveprodwestms/channelalerts/01fb2778-02c5-4247-9ac3-50169126fa47/originId     
        [Route("api/accounts/{account}/ChannelAlerts/{id}/{originId}")]
        public IEnumerable<MetricAlert> Get(string account, string id, [FromUri] AlertsQuery query, string originId = null)
        {
            var config = App.Config.GetMediaServicesSet(account);
            if (config == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            var channelAlerts = Get(config, id, query);
            if (originId != null && (query.MetricTypes == null || query.MetricTypes.Contains(MetricType.Origin)))
            {
                try
                {
                    query.MetricTypes = null;
                    var originAlerts = GetOriginAlerts(config, originId, query);
                    if(originAlerts!=null)
                        channelAlerts = channelAlerts.Concat(originAlerts);
                }catch(Exception qryEx)
                {
                    throw qryEx;
                }
            }
            return ((channelAlerts!=null)? channelAlerts : new List<MetricAlert>());
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

            var chid = channelId.NimbusIdToGuid();
            string alertstring = CloudCache.Get(chid + "Alerts");
            List<ChannelAlert> channelAlerts;
            if (alertstring != null)
            {
                channelAlerts = JsonConvert.DeserializeObject<List<ChannelAlert>>(alertstring);
            }
            else
            {
                var dataAccess = new AzureDataAccess(config.DataStorageConnections);
                channelAlerts = dataAccess.GetChannelAlerts(chid.ToString(), healthLevels, metricTypes);
                #region Not used
                //channelAlerts = dataAccess.ExecuteReadQuery(context =>
                //{
                //    var dbAlerts = context.ChannelAlerts.Where(alert =>
                //       alert.ChannelId == channelId &&
                //       alert.AlertDate >= startTime.AddTicks(alert.AlertTime.Ticks) &&
                //       alert.AlertDate <= endTime);

                //    if (healthLevels != null)
                //    {
                //        var predicate = CreatePredicate(healthLevels, "ErrorLevel");
                //        dbAlerts = dbAlerts.Where(predicate);
                //    }
                //    if (metricTypes != null)
                //    {
                //        var condition = CreatePredicate(metricTypes, "MetricType");
                //        dbAlerts = dbAlerts.Where(condition);
                //    }



                //    return dbAlerts.ToList();
                //});
                #endregion
            }
            List<MetricAlert> alerts = new List<MetricAlert>();
            if (channelAlerts != null)
            {
                foreach(var chAlt in channelAlerts)
                {
                    alerts.Add(new MetricAlert
                    {
                        AlertID = chAlt.AlertID,
                        Name = chAlt.ChannelName,
                        Date = chAlt.AlertDate.AddTicks(chAlt.AlertTime.Ticks),
                        Description = chAlt.Details,
                        Status = chAlt.ErrorLevel
                    });
                }
            }
            return alerts;
        }

        
        private IEnumerable<MetricAlert> GetOriginAlerts(MediaServicesSetConfig config, string originId, AlertsQuery query)
        {
            var controller = new OriginAlertsController();
            return controller.Get(config, originId, query);
        }
    }
}
