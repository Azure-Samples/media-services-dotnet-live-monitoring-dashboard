using System;
using System.Linq;
using NUnit.Framework;
using MediaDashboard.Persistence.Storage;
using MediaDashboard.Common;
using System.Data.Entity.Core;
using System.Collections;


namespace MediaDashboard.Persistence.Tests
{
    [TestFixture]
    public class StorageTests
    {
        AzureDataAccess _dataAccess;


        //public StorageTests()
        //{
        //    Initilialize();
        //}

        // [TestFixtureSetUp]
        //private void Initilialize()
        //{
        //    _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections);
        //}
        [Test]
        public void GetSingleEntityConnectionTest()
        {
            _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[0].DataStorageConnections);
            try
            {
                Assert.IsTrue(_dataAccess != null);
                Assert.IsTrue(_dataAccess.Connections.Count > 0);

                AMSDashboardEntities1 data = new AMSDashboardEntities1(_dataAccess.Connections[0].ConnectionString);
                Assert.IsTrue((data.Configuration != null), "Data Configuration Problem");
            }
            catch (EntityException dataEx)
            {
                Assert.Fail(dataEx.Message, dataEx);
            }
        }
        [Test]
        public void GetMultipleEntityConnectionTest()
        {
            _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections);
            try
            {
                Assert.IsTrue(_dataAccess != null);
                Assert.IsTrue(_dataAccess.Connections.Count > 1);

                AMSDashboardEntities1 data1 = new AMSDashboardEntities1(_dataAccess.Connections[0].ConnectionString);
                Assert.IsTrue((data1.Configuration != null), "Data Configuration Problem");

                AMSDashboardEntities1 data2 = new AMSDashboardEntities1(_dataAccess.Connections[0].ConnectionString);
                Assert.IsTrue((data2.Configuration != null), "Data Configuration Problem");

                Assert.IsTrue(data1 != data2);
            }
            catch (EntityException dataEx)
            {
                Assert.Fail(dataEx.Message, dataEx);
            }
        }
        [Test]
        public void GetMediaServiceAccountsTest()
        {
            try
            {
                _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections);
                var result = _dataAccess.GetAccounts();
                Assert.IsTrue((result.Count() > 0), "Data Connection Configuration Problem");
            }
            catch (EntityException dataEx)
            {
                Assert.Fail(dataEx.Message, dataEx);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

        [Test]
        public void GetDistinctDataCenterConnectionsFromMultipleConnectionsTest()
        {
            var dbCfg = App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections[1];

            _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections);
            string dbName = dbCfg.InitialCatalog;
            var found = _dataAccess.Connections.Where(cn => cn.StoreConnection.Database == dbName).FirstOrDefault();

            Assert.IsTrue((found != null), "Connection not found for this Account Name");
        }

        [Test]
        public void GetMediaChannelsByDeploymentNameTest()
        {
            var dbCfg = App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections;
            string acctName = App.Config.Content.ContentProviders[0].MediaServicesSets[1].MediaServicesAccounts[0].AccountName;

            _dataAccess = new AzureDataAccess(dbCfg);

            IEnumerable results = _dataAccess.GetChannels(acctName);
            Assert.IsNotNull(results, "Data Access Problem");
            try
            {
                Assert.IsNotEmpty(results);
            }
            catch
            {
                Assert.Inconclusive("An Empty result set is a valid response", results);
            }

        }

        [Test, Description("Test Saving Account information to a single deplyment name that is in the configuration list")]
        public void SaveMediaServiceAccountToSingleDeploymentTest()
        {
            //
            var deploymentToTest = App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections[0];

            MediaServicesAccount acct = new MediaServicesAccount
            {
                AccountName = App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections[0].AcctName,
                Location = "US West",
                SubscriptionName = "AMSLiveStream",
                AccountCreated = DateTime.Now
            };
            _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections);
            try
            {
                var acctList = _dataAccess.SaveMediaServiceAccount(acct, deploymentToTest);//, true);

                Assert.IsTrue((acctList != null), "Data Connection / Configuration Error");

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

        [Test]
        public void SaveMediaServiceAccountToMultipleDeploymentTest()
        {
            string deploymentToTest = string.Empty;
            _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections);
            var acct = App.Config.Content.ContentProviders[0].MediaServicesSets[1].MediaServicesAccounts[0];
            decimal acctCount = (decimal)_dataAccess.GetAccounts().Distinct().Count();
            //MediaServicesAccount acct = new MediaServicesAccount
            //{
            //    AccountName = App.Config.Content.ContentProviders[0].MediaServicesSets[1].DataStorageConnections[0].AcctName,
            //    Location = "US West",
            //    SubscriptionName = "AMSLiveStream",
            //    AccountCreated = DateTime.Now
            //};
            try
            {
                var acctListCount = _dataAccess.SaveMediaServiceAccount(acct);//, true);

                Assert.IsTrue((acctListCount != 0), "Data Connection / Configuration Error");
                Assert.GreaterOrEqual(acctListCount, acctCount);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }

        }

        [Test]
        public void GetChannelAlertsByAccountIDTest()
        {
            _dataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[0].DataStorageConnections);
            try
            {
                var alerts = _dataAccess.GetChannelAlerts(1);
                Assert.IsNotNull(alerts, "Data Problem found");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }

        }
    }
}
