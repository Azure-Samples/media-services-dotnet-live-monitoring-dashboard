using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Common.Metrics.MediaServices;
using MediaDashboard.Persistence.Caching;
using MediaDashboard.Persistence.Storage;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MediaDashboard.Ingest
{

    public class MonitoringWorker
    {
        private IMdCache _cache;

        private List<AzureDataConfig> _connectionList;

        //using defaults extracted from the database, and placed in the json config file
        private readonly static Dictionary<string, Common.Data.Metric> Metrics = App.Config.Parameters.Metrics;

        // We instantiate two object.
        // One is our wrapper for the Media SDK
        // The other is our data model for the cache

        public AzureMediaService MediaService { get; private set; }

        private MetricCalculator _metricCalculator = new MetricCalculator();

        private MediaService Account { get; set; }
        private TimeSpan TotalCompletionTime { get; set; }

        public AzureDataAccess DataAccess
        {
            get
            {
                return new AzureDataAccess(_connectionList);
            }
        }

        public MonitoringWorker(AzureMediaService mediaService):
            this(mediaService, App.Config.Content.ContentProviders[0].MediaServicesSets[0].DataStorageConnections)
        {
        }

        public MonitoringWorker(AzureMediaService mediaService, List<AzureDataConfig> connectionList)
        {
            connectionList.ForEach(connection =>
                Trace.TraceInformation("database:{0}  user:{1}, InitialCatalog:{2}", connection.AzureServer, connection.UserName, connection.InitialCatalog)
            );
            MediaService = mediaService;
            _connectionList = connectionList;
            _cache = MdCache.Instance;

            Account = new MediaService
            {
                Name = mediaService.Config.AccountName,
                Id = mediaService.Config.Id,
                Datacenter = mediaService.Config?.MetaData.Location,
                Origins = EntityFactory.BuildOriginListFromStreamingEndpoints(mediaService.CloudContext.StreamingEndpoints)
            };
        }

        public void Run()
        {
            RunAsync().Wait();
        }

        public async Task RunAsync()
        {
            string logmessage = string.Empty;
            DateTime loopStart = DateTime.Now;
            try
            {

                int numberOfAccounts = DataAccess.GetAccounts().Count;
                //dynamic add from the config file
                int added = DataAccess.SaveMediaServiceAccount(MediaService.Config);
                if (numberOfAccounts == 0 && added == 1)
                {
                    Trace.TraceInformation("{0} Account added to AMS Dashboard", MediaService.Config.AccountName);
                }
                else if (numberOfAccounts > 0 && added >= 1)
                {
                    Trace.TraceInformation("{0} Account Already exists in AMS Dashboard", MediaService.Config.AccountName);
                }
                else
                {
                    Trace.TraceInformation("{0} Account NOT added to AMS Dashboard, Please Check GlobalAlerts", MediaService.Config.AccountName);
                }
                try
                {
                    #region More Detailed Information
                    DateTime channelStart = DateTime.UtcNow;
                    var channels = Task.Run(() => ProcessChannels());
                    await Task.WhenAll(channels);
                    TotalCompletionTime = DateTime.UtcNow.Subtract(channelStart);
                    string completed = string.Format("{0} Channel Processing completed in {1}", MediaService.Name, TotalCompletionTime);
                    Trace.TraceInformation(completed);

                    DateTime progStart = DateTime.UtcNow;
                    var programs = Task.Run(() => ProcessPrograms());
                    await Task.WhenAll(programs);
                    TotalCompletionTime += DateTime.UtcNow.Subtract(progStart);
                    completed = string.Format("{0} Program Processing completed in {1}", MediaService.Name, DateTime.UtcNow.Subtract(progStart));
                    Trace.TraceInformation(completed);

                    DateTime orgStart = DateTime.UtcNow;
                    var origins = Task.Run(() => ProcessOrigins());
                    await Task.WhenAll(origins);
                    TotalCompletionTime += DateTime.UtcNow.Subtract(orgStart);
                    completed = string.Format("{0} Origin Processing completed in {1}", MediaService.Name, DateTime.UtcNow.Subtract(orgStart));
                    Trace.TraceInformation(completed);

                    Trace.TraceInformation("Total Completion Time for {0}: {1}", MediaService.Name, TotalCompletionTime);
                    #endregion

                    #region Less Detailed information
                    //var channels = Task.Run(() => ProcessChannels());
                    //var programs = Task.Run(() => ProcessPrograms());
                    //var origins = Task.Run(() => ProcessOrigins());
                    //await Task.WhenAll(channels, programs, origins);
                    #endregion
                }
                catch (Exception parEx)
                {
                    Trace.TraceError("Exception while processing metrics for {0}: {1}", MediaService.Name, parEx);
                }

                Account.FindChannelOriginMapping();
                _cache.SetAs(Account.Id, Account);
                Trace.TraceInformation("Added account:{0} Key:{1} to cache.", Account.Name, Account.Id);
            }
            catch (AggregateException ae)
            {
                Trace.TraceError("Exception while processing metrics for {0}: {1}", MediaService.Name, ae.Flatten());
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception while processing metrics for {0}: {1}", MediaService.Name, ex.Message, ex);
            }
            finally
            {
                var loopEnd = DateTime.Now;
                var runTime = loopEnd.Subtract(loopStart);
                string runDetails = string.Format("Account:{0} Loop execution time:{1}", MediaService.Name, runTime);
                Trace.TraceInformation(runDetails);
                if (runTime.Duration().Seconds > 30)
                {
                    MediaServicesAccount acct = DataAccess.GetAccount(MediaService.Name);
                    Alert yellowAlert = new Alert
                    {
                        AlertTypeID = 1,
                        AlertDate = DateTime.UtcNow.Date,
                        AlertTime = DateTime.UtcNow.TimeOfDay,
                        AlertValue = runTime.Seconds,
                        AccountId = acct.AccountId,
                        MetricName = "LoopExecutionTime",
                        MetricType = MetricType.Account.ToString(),
                        Details = runDetails,
                        ErrorLevel = HealthStatus.Warning.ToString()
                    };
                    DataAccess.SaveAlert(yellowAlert, acct.AccountName);
                }
                TotalCompletionTime = new TimeSpan();
            }
        }

        private void ProcessChannels()
        {
            Account.Channels = MediaService.CloudContext.Channels
                .ToList()
                .OrderBy(c => c.Name)
                .Select(EntityFactory.BuildChannelFromIChannel)
                .ToList();

            Parallel.ForEach(Account.Channels, (channel) =>
            {
                List<Alert> alerts = null;
                if (channel.State == ChannelState.Running.ToString() && MediaService.Config.TelemetryStorage != null)
                {
                    var telemetryHelper = new TelemetryHelper(MediaService.Config, channel.Id, channel.OriginId);
                    alerts = GetIngestAlerts(channel, telemetryHelper);
                    alerts.AddRange(GetArchivingAlerts(channel, telemetryHelper));
                    if (channel.EncodingType != ChannelEncodingType.None.ToString())
                    {
                        var encodingAlerts = GetEncodingAlerts(channel, telemetryHelper.GetAventusTelemetry());
                        alerts.AddRange(encodingAlerts);
                    }
                }

                PersistChannelState(channel, alerts);
            });
        }

        private void PersistChannelState(MediaChannel channel, List<Alert> channelAlerts)
        {
            try
            {
                channel.LastUpdate = DateTime.UtcNow;
                // _cache.Set(channel.Id, channel.ToString());
                Channel chnl = DataAccess.GetChannel(channel.Id.NimbusIdToRawGuid());
                if (chnl == null)
                {
                    chnl = GetNewChannel(channel);
                }
                if (channelAlerts != null && channelAlerts.Count > 0)
                {
                    DataAccess.SaveAlerts(channelAlerts, MediaService.Credentials.ClientId);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error persisting channel {0}({1}) state: {2}",  channel.Name, channel.Id, ex);
            }
        }
        private Channel GetNewChannel(MediaChannel channel)
        {
            var acct = DataAccess.GetAccount(MediaService.Credentials.ClientId);
            var chnl = new Channel
            {
                AccountId = acct.AccountId,
                ChannelId = channel.Id,
                ChannelName = channel.Name,
                Created = channel.Created,
                OriginId = Account.Origins.FindOrigin(channel.Name).Id
            };
            DataAccess.SaveChannel(chnl);
            return DataAccess.GetChannel(channel.Id.NimbusIdToRawGuid());
        }

        private Alert MapRuleToAlert(AventusHealthRule rule, string source, long ticks)
        {
            return new Alert
            {
                AlertTypeID = 2,
                AccountId = GetAccount(MediaService.Config.AccountName).AccountId,
                AlertDate = DateTime.UtcNow.Date,
                AlertTime = DateTime.UtcNow.TimeOfDay,
                MetricType = MetricType.Encoding.ToString(),
                MetricName = MetricsMapperExtensions.GetEncodingMetricName(rule.RuleID),
                ErrorLevel = rule.HealthLevel,
                Details = string.Format(
                    "{0}. Rule:{1}  Source:{2}",
                    rule.ErrorMessage,
                    rule.RuleID,
                    source)
            };
        }

        private IEnumerable<Alert> GetAlertsFromHealth(AventusHealth health, string source, ref long ticks)
        {
            var alerts = new List<Alert>();
            if (health != null && health.RuleInfo != null)
            {
                foreach (var rule in health.RuleInfo)
                {
                    alerts.Add(MapRuleToAlert(rule, source, ticks++));
                }
            }
            return alerts;
        }

        private List<Alert> GetEncodingAlerts(MediaChannel channel, AventusTelemetry telemetry)
        {
            var alerts = new List<Alert>();
            var ticks = DateTime.UtcNow.Ticks;
            if (telemetry != null)
            {
                channel.EncodingHealth = telemetry.ChannelHealth.HealthLevel.GetHealthStatus();
                var receivers = telemetry.Inputs.Receivers;
                // Add all the alerts from receivers.
                alerts.AddRange(receivers.SelectMany( (receiver, index) =>
                {
                    string source = string.Format(
                        "Receivers[{0}], TaskIndex:{1} TaskGroupId:{2}",
                        index,
                        receiver.TaskIndex,
                        receiver.TaskGroupId);
                    return GetAlertsFromHealth(receiver.Health, source, ref ticks);
                }));

                // Add all the alerts from decoders.
                var transcoders = telemetry.Transcoders;
                var decoders = transcoders.VideoDecoders;
                alerts.AddRange(decoders.SelectMany((decoder, index) =>
                {
                    var source = string.Format(
                        "Decoders[{0}], TaskIndex: {1}, TaskGroupId:{2}",
                        index,
                        decoder.TaskIndex,
                        decoder.TaskGroupId);
                    var decoderAlerts = GetAlertsFromHealth(decoder.Health, source, ref ticks);
                    var encoderAlerts = decoder.VideoEncoders.SelectMany((encoder, encoderIndex) =>
                    {
                        source = string.Format(
                            "Decoders[{0}]/Encoders[{1}], TaskIndex:{2}, TaskGroupId:{3}",
                            index,
                            encoderIndex,
                            decoder.TaskIndex,
                            decoder.TaskGroupId);
                        return GetAlertsFromHealth(decoder.Health, source, ref ticks);
                    });
                    return decoderAlerts.Concat(encoderAlerts);
                }));

                // Add all the alerts from packagers and the publishers.
                var packagers = telemetry.Outputs.Packagers;
                alerts.AddRange(packagers.SelectMany((packager, index) =>
                {
                    var source = string.Format("Packagers[{0}]", index);
                    var packagerAlerts = GetAlertsFromHealth(packager.Health, source, ref ticks);

                    var publishinerAlerts = packager.Publishers.SelectMany((publisher, pindex) =>
                    {
                        source = string.Format("Packager[{0}]/Publisher[{1}]", index, pindex);
                        return GetAlertsFromHealth(publisher.Health, source, ref ticks);
                    });
                    return packagerAlerts.Concat(publishinerAlerts);
                }));
            }
            alerts.ForEach(alert => alert.ChannelID = channel.Id);
            return alerts;
        }

        private List<Alert> GetIngestAlerts(MediaChannel channel, TelemetryHelper telemetryHelper)
        {
            List<Alert> alerts = new List<Alert>();
            var metrics = telemetryHelper.GetIngestTelemetry();
            Channel dbChannel = DataAccess.GetChannel(channel.Id);
            if (dbChannel == null)
            {
                dbChannel = GetNewChannel(channel);
            }
            long ticks = 0;
            foreach (var metric in metrics)
            {
                if (ticks == 0)
                    ticks = metric.Timestamp.Ticks;

                var healthState = metric.ComputeHealthState();

                if (healthState.Level == HealthStatus.Warning || healthState.Level == HealthStatus.Critical)
                {
                    alerts.Add(new Alert
                    {
                        AlertTypeID = 2,
                        AccountId = dbChannel.AccountId,
                        ChannelID = channel.Id.NimbusIdToRawGuid(),
                        OriginID = dbChannel.OriginId.NimbusIdToRawGuid(),
                        MetricType = MetricType.Ingest.ToString(),
                        MetricName = metric.MetricName,
                        TrackID = string.Format("{0}-{1}",metric.TrackId,metric.GroupId),
                        StreamID = string.Format("{0}_{1}",metric.StreamId,metric.Bitrate),
                        AlertValue = metric.Value,
                        ErrorLevel = healthState.Level.ToString(),
                        Details = healthState.Details,
                        AlertDate = DateTime.UtcNow.Date,
                        AlertTime = DateTime.UtcNow.TimeOfDay
                    });
                }
            };
            channel.IngestHealth = metrics
                .Select(metric => metric.ComputeHealthState().Level)
                .DefaultIfEmpty(HealthStatus.Healthy)
                .Max();
            return alerts;
        }

        private List<Alert> GetArchivingAlerts(MediaChannel channel, TelemetryHelper telemetryHelper)
        {
            List<Alert> alerts = new List<Alert>();
            var archiveMetrics = telemetryHelper.GetArchiveTelemetry();
            var acct = GetAccount(MediaService.Credentials.ClientId);
            long ticks = 0;
            foreach (var metric in archiveMetrics)
            {
                if (ticks == 0)
                    ticks = metric.Timestamp.Ticks;

                var healthState = metric.ComputeHealthState();
                if (healthState.Level == HealthStatus.Warning || healthState.Level == HealthStatus.Critical)
                {
                    alerts.Add(new Alert
                    {
                        AccountId = acct.AccountId,
                        AlertTypeID = 4,
                        ChannelID = channel.Id,
                        MetricType = MetricType.Archive.ToString(),
                        MetricName = metric.MetricName,
                        TrackID = string.Format("{0}-{1}", metric.TrackId, metric.GroupId),
                        StreamID = string.Format("{0}_{1}", metric.StreamId, metric.Bitrate),
                        AlertValue = metric.Value,
                        ErrorLevel = healthState.Level.ToString(),
                        Details = healthState.Details,
                        AlertDate = DateTime.UtcNow.Date,
                        AlertTime = DateTime.UtcNow.TimeOfDay
                    });
                }
            };

            channel.ArchiveHealth = archiveMetrics
                .Select(metric => metric.ComputeHealthState().Level)
                .DefaultIfEmpty(HealthStatus.Healthy)
                .Max();
            return alerts;
        }


        private void ProcessPrograms()
        {
            Account.Programs = MediaService.CloudContext.Programs
                .ToList()
                .Select(program =>
                {
                    var programDetails = EntityFactory.BuildProgramFromIProgram(program);
                    return programDetails;
                })
                .ToList();
            foreach (var program in Account.Programs)
            {
                PersistProgramState(program);
            }
        }

        private void PersistProgramState(MediaProgram prg)
        {
            try
            {
                prg.LastMetricUpdate = DateTime.UtcNow;
                // _cache.Set(prg.Id, prg.ToString());
                Program prgrm = DataAccess.GetProgram(prg.Id);
                if (prgrm == null)
                {
                    prgrm = GetNewProgram(prg);
                }

                //todo: Get Program Telemetry  and save it here
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error persisting program {0}({1}) state: {2}", prg.Name, prg.Id, ex);
            }
        }

        private Program GetNewProgram(MediaProgram prg)
        {
            try
            {
                Program program = new Program
                {
                    ProgramID = prg.Id,
                    ChannelID = prg.ChannelId.NimbusIdToRawGuid(),
                    ProgramName = prg.Name,
                    Created = prg.Created
                };
                return DataAccess.SaveProgram(program, MediaService.Credentials.ClientId);
            }
            catch
            {
                throw;
            }

        }

        private void ProcessOrigins()
        {
            Account.Origins = MediaService.CloudContext.StreamingEndpoints
                .ToList()
                .OrderBy(o => o.Name)
                .Select(EntityFactory.BuildOriginFromIStreamingEndpoint)
                .ToList();
            Parallel.ForEach(Account.Origins, (origin) =>
            {
                List<Alert> alerts = null;
                if (origin.State == StreamingEndpointState.Running.ToString() && MediaService.Config.TelemetryStorage != null)
                {
                    alerts = GetOriginAlerts(origin);
                }
                PersistOriginState(origin, alerts);
            });
        }

        private void PersistOriginState(MediaOrigin origin, List<Alert> originAlerts)
        {
            string acctName = MediaService.Credentials.ClientId;
            var acct = GetAccount(MediaService.Credentials.ClientId);
            var dbOrigin = DataAccess.GetOrigin(origin.Id);

            if (dbOrigin == null)
            {
                dbOrigin = new Origin
                {
                    OriginId = origin.Id.NimbusIdToRawGuid(),
                    AccountId = DataAccess.GetAccount(acctName).AccountId,
                    OriginName = origin.Name,
                    Created = origin.Created
                };
                DataAccess.SaveOrigin(dbOrigin, acctName);
            }

            if (originAlerts != null && originAlerts.Count > 0)
            {
                DataAccess.SaveAlerts(originAlerts, acctName);
            }
        }
        private MediaServicesAccount GetAccount(string acctName)
        {
            return DataAccess.GetAccount(acctName);
        }

        private List<Alert> GetOriginAlerts(MediaOrigin origin)
        {
            var alerts = new List<Alert>();
            if (MediaService.Config.TelemetryStorage != null)
            {
                var telemetryHelper = new TelemetryHelper(MediaService.Config, null, origin.Id);
                var metrics = telemetryHelper.GetOriginTelemetry(origin.ReservedUnits);
                var account = GetAccount(MediaService.Credentials.ClientId);
                long ticks = 0;
                foreach (var metric in metrics)
                {
                    if (ticks == 0)
                        ticks = metric.Timestamp.Ticks;

                    var healthState = metric.ComputeHealthState();
                    if (healthState.Level == HealthStatus.Warning || healthState.Level == HealthStatus.Critical)
                    {
                        alerts.Add(new Alert
                        {
                            AlertTypeID = 3,
                            AccountId = account.AccountId,
                            OriginID = origin.Id,
                            MetricType = MetricType.Origin.ToString(),
                            MetricName = metric.Metric.DisplayName,
                            AlertValue = metric.Value,
                            ErrorLevel = healthState.Level.ToString(),
                            Details = healthState.Details,
                            AlertDate = DateTime.UtcNow.Date,
                            AlertTime = DateTime.UtcNow.TimeOfDay
                        });
                    }
                };
            }

            if (alerts.Count > 0)
            {
                origin.Health = (HealthStatus)alerts.Max(alert => Enum.Parse(typeof(HealthStatus), alert.ErrorLevel));
            }
            else
            {
                origin.Health = HealthStatus.Healthy;
            }

            return alerts;
        }

    }
}
