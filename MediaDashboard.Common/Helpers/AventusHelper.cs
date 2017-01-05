using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Common.TelemetryStorageClient;
using Microsoft.WindowsAzure.MediaServices.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace MediaDashboard.Common.Data
{
    public class AventusHelper
    {
        #region Fields & Properties
        X509Certificate2 Certificate { get; set; }

        public HttpClient TelemetryClient { get; private set; }

        private TelemetryStorage TelemetryStorage { get; set; }

        private MediaServicesAccountConfig AccountConfig { get; set; }

        public string AventusDNSTail { get; private set; }//Static url base for Aventus Telemetry Calls
        #endregion
        #region ctors

        public AventusHelper():
            this(App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0])
        {
        }

        public AventusHelper(MediaServicesAccountConfig config)
        {
            AccountConfig = config;
            if(config.TelemetryStorage != null)
            {
                TelemetryStorage = new TelemetryStorage(config.TelemetryStorage);
            }
            if (config.MetaData.Thumbprint != null)
            {
                AventusDNSTail = config.MetaData.aventusDNSBase;
                Certificate = ChannelCreationOperations.GetCertificate(config.MetaData.Thumbprint);
                TelemetryClient = ChannelCreationOperations.GetWebClient(null, Certificate);
            }
        }

        #endregion

        public AventusChannel GetChannelInfo(IChannel channel)
        {
            if (TelemetryStorage != null)
            {
                var channelInfo = new AventusChannel();
                channelInfo.TelemetryResult = GetTelemetryInfo(channel);
                channelInfo.Status = channelInfo.TelemetryResult == null ? channel.State.ToString() : "OnAir";
                return channelInfo;
            }
            else
            {
                return GetChannelInfoFromRest(channel);
            }
        }

        private AventusChannel GetChannelInfoFromRest(IChannel channel)
        {
            AventusChannel channelInfo = BuildAventusChannel(channel, AccountConfig.AccountName);

            if (channelInfo != null)
            {
                
                switch (channelInfo.Status.ToLower())
                {
                    case "running":
                    case "onair":
                    case "ready":
                        string telem = GetTelemetryInfo(channelInfo.BaseUrl);
                        channelInfo.TelemetryResult = (!string.IsNullOrEmpty(telem) ? JsonConvert.DeserializeObject<AventusTelemetry>(telem) : null);
                        channelInfo.RunId = ((channelInfo.TelemetryResult != null) ? channelInfo.TelemetryResult.RunId : Guid.NewGuid().ToString());
                        if (channelInfo.TelemetryResult != null)
                        {
                            channelInfo.TelemetryResult.ChannelName = channel.Name;
                        }
                        break;
                    default:
                        channelInfo.TelemetryResult = null;
                        break;
                }
                string status = string.Format("{0} ({1})", channel.State.ToString(), channelInfo.Status);
                channelInfo.Status = status;
            }
            return channelInfo;
        }

        private string GetHttpResponse(string url)
        {
            try
            {
                var response = TelemetryClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch(AggregateException aex)
            {
                Trace.TraceError("Error getting response for {0}. {1}", url, aex);
            }
            return null;
        }

        private string GetTelemetryInfo(string baseUrl)
        {
            string telemAddress = (string.Format(@"{0}/telemetry", baseUrl));
            return GetHttpResponse(telemAddress);
        }

        public AventusTelemetry GetTelemetryInfo(IChannel channel)
        {
            AventusTelemetry telemetry = null;
            if (AccountConfig.TelemetryStorage != null)
            {
                telemetry = GetTelemetryFromStorage(channel);
            }
            
            // temporary workaround for reset bug. fallback to REST.
            if(telemetry == null && Certificate != null)
            {
                telemetry = GetTelemetryFromRest(channel);
            }
            return telemetry;
        }

        private AventusTelemetry GetTelemetryFromRest(IChannel channel)
        {
            AventusChannel channelInfo = BuildAventusChannel(channel, AccountConfig.AccountName);
            if (channelInfo != null)
            {
                string telemResult = GetTelemetryInfo(channelInfo.BaseUrl);
                if (!string.IsNullOrEmpty(telemResult))
                {
                    AventusTelemetry retVal = JsonConvert.DeserializeObject<AventusTelemetry>(telemResult);
                    retVal.ChannelName = channel.Name;
                    return retVal;
                }
            }
            return null;
        }

        private AventusTelemetry GetTelemetryFromStorage(IChannel channel)
        {
            var telemetryHelper = new TelemetryHelper(AccountConfig, channel);
            return telemetryHelper.GetAventusTelemetry();
        }

        private AventusChannel BuildAventusChannel(IChannel channel, string mediaSvcAcctName)
        {
            string aventusApiBase = BuildAventusDNSBase(channel.Name, mediaSvcAcctName);
            string aventusChannelAddress = (string.Format(@"{0}/api/channels", aventusApiBase));
            AventusChannel channelInfo = null;

            switch (channel.State)
            {
                case ChannelState.Running:
                    //there is a separate Aventus GUID separate from AMS channel GUID.
                    var result = GetHttpResponse(aventusChannelAddress);
                    if(string.IsNullOrEmpty(result))
                    {
                        break;
                    }

                    var channels = JsonConvert.DeserializeObject<AventusChannelList>(result);
                    if (channels.ChannelList.Length == 0)
                    {
                        throw new WebException("Channel not in correct state!");
                    }
                    channelInfo = channels.ChannelList[0];
                    channelInfo.AventusChannelId = GetAventusChannelId(channelInfo.BaseUrl);
                    break;
            }
            return channelInfo;
        }

        private string GetAventusChannelId(string baseUrl)
        {
            Uri aoaLoc = new Uri(baseUrl);
            return aoaLoc.Segments.Last();
        }

        private string BuildAventusDNSBase(string channelName, string accountName)
        {
            return string.Format("https://{0}-{1}{2}", channelName, accountName, AventusDNSTail);
        }
    
    }
}
