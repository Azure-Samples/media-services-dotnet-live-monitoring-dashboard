using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace MediaDashboard.Persistence.Storage
{

    public class AzureDataAccess
    {
        #region Properties
        /// <summary>
        /// Tihs is public to ensure propoer test coverage, and datbase configuration
        /// </summary>
        public List<EntityConnection> Connections { get; private set; }

        #endregion

        #region ctor and Connections

        public AzureDataAccess(List<AzureDataConfig> connectionList)
        {
            Connections = connectionList.Select(connection => GetConnection(connection)).ToList();
        }


        private EntityConnection GetConnection(AzureDataConfig config)
        {
            EntityConnectionStringBuilder bldr = new EntityConnectionStringBuilder();
            //bldr.Metadata = "res://*/Storage.WAMSData.csdl|res://*/Storage.WAMSData.ssdl|res://*/Storage.WAMSData.msl";
            //metadata=res://*/Storage.AMSDashboard.csdl|res://*/Storage.AMSDashboard.ssdl|res://*/Storage.AMSDashboard.msl;provider=System.Data.SqlClient;provider connection string="data source=AMCFALL\SQL2014;initial catalog=AMSDashboard;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework"
            bldr.Metadata = "res://*/Storage.AMSDashboard.csdl|res://*/Storage.AMSDashboard.ssdl|res://*/Storage.AMSDashboard.msl";
            bldr.Provider = "System.Data.SqlClient";
            bldr.ProviderConnectionString = config.BasicConnectionString;
            return new EntityConnection(bldr.ConnectionString);
        }
        #endregion

        #region Generic Functions
        public T ExecuteReadQuery<T>(Func<AMSDashboardEntities1, T> query)
        {
            foreach (var connection in Connections)
            {
                try
                {
                    using (var dataContext = new AMSDashboardEntities1(connection.ConnectionString))
                    {
                        return query(dataContext);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Database read error: {0}.", ex);
                    Trace.TraceInformation("SQL error trying the read. continue to next connection");
                }
            }
            return default(T);
        }



        public void ExecuteWriteQuery(Action<AMSDashboardEntities1> query)
        {
            foreach (var connection in Connections)
            {
                try
                {
                    using (var dataContext = new AMSDashboardEntities1(connection.ConnectionString))
                    {
                        query(dataContext);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Database write error: {0}", ex);
                    Trace.TraceInformation("SQL error trying to write. Continue to the next connection");
                }
            }
        }

        private void ExecuteWriteQuery(Action<AMSDashboardEntities1> query, string acctName)
        {
            var accts = GetAccounts(acctName);
            foreach (var connection in Connections)
            {
                try
                {
                    using (var dataContext = new AMSDashboardEntities1(connection.ConnectionString))
                    {
                        query(dataContext);
                    }
                }
                catch (Exception ex)
                {
                    string errorDetails = string.Format("Database write error: {0}.", ex.Message + ex.StackTrace);
                    Trace.TraceError("Database write error: {0}", ex);
                    Trace.TraceInformation("SQL error trying to write. Continue to the next connection");
                    foreach (MediaServicesAccount ac in accts)
                    {
                        ExecuteWriteQuery(dataContext =>
                        {
                            Alert alert = new Alert
                            {
                                AlertTypeID = 1,
                                AlertDate = DateTime.UtcNow,
                                AlertTime = DateTime.UtcNow.TimeOfDay,
                                AccountId = ac.AccountId,
                                MetricType = MetricType.Database.ToString(),
                                MetricName = "Database write error",
                                ErrorLevel = "Critical",
                                Details = errorDetails
                            };
                        }, acctName);
                    }
                }
            }
        }



        public List<GlobalAlert> GetAccountGlobalAlerts(string name)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dc) => dc.GlobalAlerts
                .Where(ga => ga.AccountName == name)
                .OrderByDescending(alt => alt.AlertID)
                .ToList());
        }
        public List<GlobalAlert> GetGlobalAlerts(string acctName)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dataContext) => dataContext
               .GlobalAlerts
                .Where(ga => ga.AccountName == acctName)
                .OrderByDescending(alt => alt.AlertID)
                .ToList());
        }
        #endregion

        public List<MediaServicesAccount> GetAccounts()
        {
            return ExecuteReadQuery((dataContext) => dataContext.MediaServicesAccounts.ToList());
        }

        public List<MediaServicesAccount> GetAccounts(string subscription)
        {
            return ExecuteReadQuery((dataContext) => dataContext
                    .MediaServicesAccounts
                    .Where(acct => acct.SubscriptionName == subscription)
                    .ToList());
        }

        public List<MediaServicesAccount> SaveMediaServiceAccount(MediaServicesAccount acct, AzureDataConfig deploymentToTest)
        {
            using(var dataContext = new AMSDashboardEntities1(GetConnection(deploymentToTest).ConnectionString))
            {
                dataContext.MediaServicesAccounts.Add(acct);
                dataContext.SaveChanges();
                return GetAccounts();
            }
        }

        public MediaServicesAccount GetAccount(string name)
        {
            return ExecuteReadQuery((dataContext) => dataContext
                   .MediaServicesAccounts
                   .Where(acct => acct.AccountName == name)
                   .FirstOrDefault());
        }



        public List<Origin> GetOrigins(int acctid)
        {
            return ExecuteReadQuery((dataContext) => dataContext.Origins
                    .Where(org => org.AccountId == acctid)
                    .ToList());
        }

        public Channel GetChannel(string chid)
        {
            return ExecuteReadQuery((dataContext) => dataContext.Channels
                     .Where(ch => ch.ChannelId == chid)
                     .FirstOrDefault());
        }

        public void SaveChannel(string acctName, Channel chnl)
        {
            ExecuteWriteQuery(dataContext =>
            {
                int added = 0;
                var channel = dataContext.Channels.Where(ch => ch.ChannelId == chnl.ChannelId).FirstOrDefault();
                if (channel == null)
                {
                    dataContext.Channels.Add(chnl);
                    added = dataContext.SaveChanges();
                }
            }, acctName); ;
        }
        public void SaveChannel(Channel chnl)
        {
            int added = 0;
            ExecuteWriteQuery(dataContext =>
            {
                dataContext.Channels.Add(chnl);
                added = dataContext.SaveChanges();
            });
        }
        public List<Channel> GetChannels(string acctName)
        {
            return ExecuteReadQuery((dataContext) => dataContext.Channels
                    .Where(ch => ch.MediaServicesAccount.AccountName == acctName)
                    .ToList());
        }

        public int SaveMediaServiceAccount(MediaServicesAccountConfig config)
        {
            int added = 0;
            MediaServicesAccount newAcct = new MediaServicesAccount
            {
                AccountName = config.AccountName,
                SubscriptionName = config.MetaData.AzureSubscriptionId,
                DataCenter=config.MetaData.ResourceGroup,
                Location = config.MetaData.Location,
                AccountCreated = DateTime.UtcNow.Date
            };

            ExecuteWriteQuery(dataContext =>
            {
                //check for the existing account first
                var acct = dataContext.MediaServicesAccounts
                    .FirstOrDefault(account => account.AccountName == config.AccountName);
                if (acct == null)
                {
                    //only add if the account is not in the db
                    dataContext.MediaServicesAccounts.Add(newAcct);
                    added = dataContext.SaveChanges();
                }
                else
                    added = acct.AccountId;
            });
            return added;
        }

        public List<Program> GetPrograms(string chid)
        {
            return ExecuteReadQuery((dataContext) => dataContext.Programs
                    .Where(prg => prg.ChannelID == chid)
                    .ToList());
        }

        public List<ChannelAlert> GetChannelAlerts(int acctid)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dataContext) => dataContext.ChannelAlerts
                    .Where(ch => ch.AccountId == acctid)
                    .OrderByDescending(alt => alt.AlertID)
                    .ToList());
        }

        public List<ChannelAlert> GetChannelAlerts(string channelId)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dataContext) => dataContext.ChannelAlerts
                    .Where(ch => ch.ChannelId == channelId)
                    .OrderByDescending(alt=>alt.AlertID)
                    .ToList());
        }
        public List<ChannelAlert> GetChannelAlerts(string chid, HealthStatus[] healthLevels, MetricType[] metricTypes)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dc) => dc.ChannelAlerts
            .Where(alt =>
                alt.ChannelId == chid &&
                alt.ErrorLevel == ((healthLevels != null) ? healthLevels.Max().ToString() : "") ||
                alt.MetricType.Equals(((metricTypes != null) ? metricTypes.Any().ToString() : ""))
            )
            .OrderByDescending(alt => alt.AlertID)
            .ToList());
        }
        public List<OriginAlert> GetOriginAlerts(int acctid)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dataContext) => dataContext.OriginAlerts
                    .Where(org => org.AccountId == acctid)
                    .OrderByDescending(alt => alt.AlertID)
                    .ToList());
        }
        public List<OriginAlert> GetOriginAlerts(string origId)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dc)=>dc.OriginAlerts
                .Where(alt =>
                    origId.Contains(alt.OriginID)
            )
            .OrderByDescending(alt => alt.AlertID)
            .ToList());
        }
        public List<OriginAlert> GetOriginAlerts(string origId, HealthStatus[] healthLevels)
        {
            //returns the list for the past 30 seconds
            return ExecuteReadQuery((dc) => dc.OriginAlerts
                .Where(alt =>
                    origId.Contains(alt.OriginID) &&
                    alt.ErrorLevel == ((healthLevels != null) ? healthLevels.Max().ToString() : "")
                )
                .OrderByDescending(alt => alt.AlertID)
                .ToList());
        }
        public void SaveAlert(Alert alert, string acctName)
        {
            int added = 0;
            ExecuteWriteQuery((dataContext) =>
            {
                dataContext.Alerts.Add(alert);
                added = dataContext.SaveChanges();
            }, acctName);
        }
        public void SaveAlerts(List<Alert> alerts, string acctName)
        {
            int added = 0;
            
            ExecuteWriteQuery((dataContext) =>
            {
                try
                {
                    dataContext.Alerts.AddRange(alerts);
                    added = dataContext.SaveChanges();
                }catch(DbEntityValidationException dbValEx)
                {
                    throw dbValEx;
                }
            }, acctName);
        }

        public Program GetProgram(string prgid)
        {
            return ExecuteReadQuery((dataContext) => dataContext.Programs
                    .Where(pr => pr.ProgramID == prgid)
                    .FirstOrDefault());
        }


        public Program SaveProgram(Program program, string acctName)
        {
            int added = 0;
            ExecuteWriteQuery((dataContext) =>
            {
                dataContext.Programs.Add(program);
                added = dataContext.SaveChanges();
            }, acctName);

            return GetProgram(program.ProgramID);

        }

        public Origin GetOrigin(string originId)
        {
            return ExecuteReadQuery((dataContext) => dataContext.Origins
                .Where(org => org.OriginId == originId)
                .FirstOrDefault());
        }

        public void SaveOrigin(Origin dbOrigin, string acctName)
        {
            int added = 0;
            ExecuteWriteQuery((dataContext) =>
            {
                dataContext.Origins.Add(dbOrigin);
                added = dataContext.SaveChanges();
            }, acctName);
        }

        public int CleanOutOldData(DateTime earliestDate)
        {
            var alerts = ExecuteReadQuery((dc)=>dc.Alerts.Where(alt => alt.AlertDate < earliestDate).ToList());
            int cleanedOut = 0;  //alerts.Count;
            if (alerts !=null && alerts.Count > 0)
            {
                ExecuteWriteQuery((dc) =>
                {
                    dc.Alerts.RemoveRange(alerts);
                    dc.SaveChanges();
                });
            }
            return cleanedOut;
        }


    }
}
