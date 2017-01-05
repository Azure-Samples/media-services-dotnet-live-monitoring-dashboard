namespace MediaDashboard.MediaServices.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    /// <summary>
    /// This class is based on the use of the app config to capture the settings
    /// </summary>
    [TestClass]
    public class MediaContextBaseExtensionsFixture
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
        }

        [TestMethod]
        public void ShouldCreateAzureTableNotificationEndPoint()
        {
            var context = CreateContext();

            var name = "monitoring";
            var endPointType = NotificationEndPointTypeV212.AzureTable;
            var endPointAddress = context.GetAzureTableEndPointAddress();

            var notificationEndPoint = context.GetNotificationEndPoints().Create(name, endPointType, endPointAddress);

            Assert.IsNotNull(notificationEndPoint);
            Assert.IsNotNull(notificationEndPoint.Id);
            Assert.AreEqual(name, notificationEndPoint.Name);
            Assert.AreEqual(endPointType, notificationEndPoint.EndPointType);
            Assert.AreEqual(endPointAddress, notificationEndPoint.EndPointAddress);

            notificationEndPoint = context.GetNotificationEndPoints().Where(ne => ne.Id == notificationEndPoint.Id).FirstOrDefault();

            Assert.IsNotNull(notificationEndPoint);
            Assert.IsNotNull(notificationEndPoint.Id);
            Assert.AreEqual(name, notificationEndPoint.Name);
            Assert.AreEqual(endPointType, notificationEndPoint.EndPointType);
            Assert.AreEqual(endPointAddress, notificationEndPoint.EndPointAddress);

            notificationEndPoint.Delete();

            Assert.IsNull(context.GetNotificationEndPoints().Where(ne => ne.Id == notificationEndPoint.Id).FirstOrDefault());
        }

        [TestMethod]
        public void ShouldGetServiceMetadata()
        {
            var context = CreateContext();

            var serviceMetadata = context.GetServiceMetadata();

            Assert.IsNotNull(serviceMetadata);
            Assert.IsTrue(serviceMetadata.IsMonitoringEnabled);
        }

        [TestMethod]
        public void ShouldGetMonitoringSettingsCollection()
        {
            var context = CreateContext();

            var monitoringSettingsCollection = context.GetMonitoringSettings();

            Assert.IsNotNull(monitoringSettingsCollection);

            var monitoringSettings = monitoringSettingsCollection.ToList();

            Assert.IsNotNull(monitoringSettings);
        }

        [TestMethod]
        public void ShouldCreateMonitoringSetting()
        {
            var context = CreateContext();

            var notificationEndPoint = context.GetNotificationEndPoints().Create("monitoring", NotificationEndPointTypeV212.AzureTable, context.GetAzureTableEndPointAddress());
            var settings = new List<ComponentMonitoringSettings>
            {
                new ComponentMonitoringSettings
                {
                    Component = "Channel",
                    Level = "Normal"
                },
                new ComponentMonitoringSettings
                {
                    Component = "StreamingEndpoint",
                    Level = "Normal"
                }
            };

            var monitoringSettings = context.GetMonitoringSettings().Create(notificationEndPoint.Id, settings);

            Assert.IsNotNull(monitoringSettings);
            Assert.AreEqual(notificationEndPoint.Id, monitoringSettings.NotificationEndPointId);
            Assert.IsNotNull(monitoringSettings.Settings);
            Assert.AreEqual(settings.Count, monitoringSettings.Settings.Count);
            Assert.AreEqual(settings[0].Component, monitoringSettings.Settings[0].Component);
            Assert.AreEqual(settings[0].Level, monitoringSettings.Settings[0].Level);
            Assert.AreEqual(settings[1].Component, monitoringSettings.Settings[1].Component);
            Assert.AreEqual(settings[1].Level, monitoringSettings.Settings[1].Level);

            monitoringSettings.Delete();
            notificationEndPoint.Delete();
        }

        [TestMethod]
        public void ShouldCreateOrUpdateMonitoringSettings()
        {
            var context = CreateContext();
            var level = "Normal";

            var monitoringSettings = context.CreateOrUpdateMonitoringSettings(level);

            Assert.IsNotNull(monitoringSettings);
            Assert.IsNotNull(monitoringSettings.NotificationEndPointId);
            Assert.IsNotNull(monitoringSettings.Settings);
            Assert.AreEqual(2, monitoringSettings.Settings.Count);
            Assert.AreEqual("Channel", monitoringSettings.Settings[0].Component);
            Assert.AreEqual(level, monitoringSettings.Settings[0].Level);
            Assert.AreEqual("StreamingEndpoint", monitoringSettings.Settings[1].Component);
            Assert.AreEqual(level, monitoringSettings.Settings[1].Level);

            level = "Verbose";

            monitoringSettings = context.CreateOrUpdateMonitoringSettings(level);

            Assert.IsNotNull(monitoringSettings);
            Assert.IsNotNull(monitoringSettings.NotificationEndPointId);
            Assert.IsNotNull(monitoringSettings.Settings);
            Assert.AreEqual(2, monitoringSettings.Settings.Count);
            Assert.AreEqual("Channel", monitoringSettings.Settings[0].Component);
            Assert.AreEqual(level, monitoringSettings.Settings[0].Level);
            Assert.AreEqual("StreamingEndpoint", monitoringSettings.Settings[1].Component);
            Assert.AreEqual(level, monitoringSettings.Settings[1].Level);

            var notificationEndPoint = context.GetNotificationEndPoints().Where(ne => ne.Id == monitoringSettings.NotificationEndPointId).First();

            monitoringSettings.Delete();
            notificationEndPoint.Delete();
        }

        private static MediaContextBase CreateContext()
        {
            var accountName = ConfigurationManager.AppSettings["MediaServicesAccountName"];
            var accountKey = ConfigurationManager.AppSettings["MediaServicesAccountKey"];
            var apiServer = ConfigurationManager.AppSettings["MediaServicesAccountApiUrl"];
            var scope = ConfigurationManager.AppSettings["MediaServicesAccountScope"];
            var acsBaseAddress = ConfigurationManager.AppSettings["MediaServicesAccountAcsBaseAddress"];

            if (!string.IsNullOrWhiteSpace(apiServer) && !string.IsNullOrWhiteSpace(scope) && !string.IsNullOrWhiteSpace(acsBaseAddress))
            {
                return new CloudMediaContext(new Uri(apiServer), new MediaServicesCredentials(accountName, accountKey, scope, acsBaseAddress));
            }

            var credentials = new MediaServicesCredentials(accountName, accountKey);
            if (!string.IsNullOrWhiteSpace(apiServer) )
            {
                return new CloudMediaContext(new Uri(apiServer), credentials);
            }

            return new CloudMediaContext(credentials);
        }
    }
}
