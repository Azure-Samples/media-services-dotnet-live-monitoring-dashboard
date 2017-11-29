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
    public class ChannelAlertsController : AlertsController
    {
        //accounts/nbcsgliveprodwestms/channelalerts/01fb2778-02c5-4247-9ac3-50169126fa47/originId     
        [Route("{id}/{originId}")]
        public IActionResult Get(string account, string id, [FromQuery] AlertsQuery query, string originId = null)
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

            var channelAlerts = Get(config, id, query);
            if (originId != null && (query.MetricTypes == null || query.MetricTypes.Contains(MetricType.Origin)))
            {
                try
                {
                    query.MetricTypes = null;
                    var originAlerts = GetOriginAlerts(config, originId, query);
                    if(originAlerts!=null)
                        channelAlerts = channelAlerts.Concat(originAlerts);
                }
                catch (Exception qryEx)
                {
                    throw qryEx;
                }
            }

            var alerts =  channelAlerts ?? new List<MetricAlert>();
            return Ok(alerts);
        }

        private IEnumerable<MetricAlert> Get(MediaServicesSetConfig  config, string channelId, AlertsQuery query)
        {
            var chid = channelId.NimbusIdToGuid();
            var channelAlerts = GetAlertsFromCache<ChannelAlert>(chid.ToString());
            if (channelAlerts == null)
            {
                var dataAccess = new AzureDataAccess(config.DataStorageConnections);
                channelAlerts = dataAccess.GetChannelAlerts(chid.ToString(), query.StatusLevels, query.MetricTypes);
            }

            
            IEnumerable<MetricAlert> alerts = null;
            if (channelAlerts != null)
            {
                alerts  = channelAlerts.Select(chAlt => new MetricAlert
                    {
                        AlertID = chAlt.AlertID,
                        Name = chAlt.ChannelName,
                        Date = chAlt.AlertDate.AddTicks(chAlt.AlertTime.Ticks),
                        Description = chAlt.Details,
                        Status = chAlt.ErrorLevel
                    });
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
