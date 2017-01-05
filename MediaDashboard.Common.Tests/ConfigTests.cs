using System.Diagnostics;
using NUnit.Framework;

namespace MediaDashboard.Common.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void CanReadDevConfig()
        {
            // Read some values to make sure it works
            Assert.IsNotNull(App.Config.Sys.Cache.CacheName, "Name of cache is not defined");
            Assert.IsNotNull(App.Config.Sys.Cache.LocalCacheDir, "Local cache dir is not defined");
            Assert.IsNotNull(App.Config.Content.DashboardTitle, "Dashboard title is not defined");
            Assert.IsTrue((App.Config.Content.ContentProviders.Count >0), "List of Content Providers is not defined");
            Assert.IsNotNull(App.Config.Content.ContentProviders[0].Name, "Name of first content provider not defined");
            Assert.AreEqual(160, App.Config.Parameters.ReservedUnitConfig.Capacity, "Capacity value is incorrect");
            Assert.AreEqual(50, App.Config.Parameters.Origin.Thresholds.Warning, "Warning value for Origin is incorrect");
            Assert.AreEqual(70, App.Config.Parameters.Origin.Thresholds.Error, "Error value for Origin is incorrect");
            
            // Not all values are tested but add if we need more tests
        }

        [Test]
        public void CanOverrideDefaultConfigBehavior()
        {
            Debug.WriteLine(App.Config.Content.DashboardTitle);
        }

        [Test]
        public void CanReadDevDataConn()
        {
            var dataConfig = App.Config.Content.ContentProviders[0].MediaServicesSets[0].DataStorageConnections[0];
            Assert.IsNotNull(dataConfig, "Data Storage Connection Not Found");
            Assert.IsNotNull(dataConfig.AcctName, "Account Name is not defined");
            Assert.IsNotNull(dataConfig.AzureServer, "Database Server is not defined");
            Assert.IsNotNull(dataConfig.InitialCatalog, "Database Name is not defined");
            Assert.IsNotNull(dataConfig.UserName, "Database UserName is not defined");
            Assert.IsNotNull(dataConfig.Password, "Database UserName is not defined");
            Assert.IsNotNull(dataConfig.BasicConnectionString, "ConnectionString not complete");
        }

        [Test]
        public void CanReadMetricThresholds()
        {
            var appConfig = App.Config;
            var defaultThresholds = appConfig.Parameters.MetricThresholds;

            Assert.IsNotNull(defaultThresholds, "Default Metric Thresholds not found!");
            Assert.IsTrue((defaultThresholds.Length > 0), "Default Metric Thresholds not defined!");
        }
    }
}