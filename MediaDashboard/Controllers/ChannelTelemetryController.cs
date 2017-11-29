using System.Linq;
using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.AspNetCore.Mvc;

namespace MediaDashboard.Controllers
{
    public class ChannelTelemetryController : ControllerBase
    {
        // GET: /accounts/accountName/ChannelTelemetry/channelId
        public IActionResult Get(string account, string id)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                return NotFound();
            }
            return Ok(Get(accountConfig, id));
        }

        private IActionResult Get(MediaServicesAccountConfig accountConfig, string id)
        {
            var chid = string.Format("nb:chid:UUID:{0}", id);
            AventusTelemetry telemetry = null;

            var context = accountConfig.GetContext();
            var channel = context.Channels.Where(ch => ch.Id == chid).FirstOrDefault();
            if (channel == null)
            {
                return NotFound();
            }

            if (channel.EncodingType != ChannelEncodingType.None)
            {
                var aventusHelper = new AventusHelper(accountConfig);
                telemetry = aventusHelper.GetTelemetryInfo(channel);
            }
            return Ok(telemetry);
        }
    }
}
