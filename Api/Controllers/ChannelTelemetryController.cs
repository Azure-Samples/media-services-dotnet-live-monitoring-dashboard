using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using Microsoft.WindowsAzure.MediaServices.Client;
using System.Linq;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Controllers
{
    public class ChannelTelemetryController : ControllerBase
    {

        public ChannelTelemetryController() : base() 
        {
        }

        // GET: api/ChannelTelemetry/5
        public AventusTelemetry Get(string id)
        {
            return Get(DefaultAccount, id);
        }

        // GET: /accounts/accountName/ChannelTelemetry/channelId
        public AventusTelemetry Get(string account, string id)
        {
            var accountConfig = App.Config.GetMediaServicesAccount(account);
            if (accountConfig == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            return Get(accountConfig, id);
        }

        private AventusTelemetry Get(MediaServicesAccountConfig accountConfig, string id)
        {
            var chid = string.Format("nb:chid:UUID:{0}", id);
            AventusTelemetry telemetry = null;

            var context = accountConfig.GetContext();
            var channel = context.Channels.Where(ch => ch.Id == chid).FirstOrDefault();
            if (channel == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            if (channel.EncodingType != ChannelEncodingType.None)
            {
                var aventusHelper = new AventusHelper(accountConfig);
                telemetry = aventusHelper.GetTelemetryInfo(channel);
            }
            return telemetry;
        }
    }
}
