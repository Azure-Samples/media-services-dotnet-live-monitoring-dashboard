using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDashboard.Ingest.Tests
{
    using MediaDashboard.Ingest;
    using MediaDashboard.Common;
    using System.Net;
    using MediaDashboard.Common.Config.Entities;


    [TestClass]
    public class UnitTest1
    {
        List<MediaServicesSetConfig> _svcSetConfigs;
        
        [TestMethod, Ignore]
        public void MonitoringControllerTest1()
        {
            List<AzureMediaService> _mediaServices = new List<AzureMediaService>();

            // init 
            AzureMediaService testMediaServices1 = new AzureMediaService("ENTER MEDIA ACCOUNT NAME 1", "ENTER MEDIA ACCOUNT KEY 1");
            AzureMediaService testMediaServices2 = new AzureMediaService("ENTER MEDIA ACCOUNT NAME 2", "ENTER MEDIA ACCOUNT KEY 2");

            testMediaServices1.Id = @"ENTER MEDIA SERVICE ACCOUNT ID 1";
            testMediaServices2.Id = @"ENTER MEDIA SERVICE ACCOUNT ID 2";

            _mediaServices.Add(testMediaServices1);
            _mediaServices.Add(testMediaServices2);

            MediaDashboard.Ingest.MonitoringWorker mw = new MonitoringWorker(_mediaServices[0]);
            mw.Run();
        }

        [TestMethod, Ignore]
        public void MonitoringControllerTest2()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            WebRequest.DefaultWebProxy = new WebProxy("http://localhost:8888/", false);
            try
            {
               // MonitoringWorker mw = new MonitoringWorker(svc);
                MonitoringController mw = new MonitoringController();
                mw.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

         [TestMethod, Ignore]
        public void StartFromControllerTest()
        {
            AzureMediaService svc=null;
            Dictionary<string, object> errorList = new Dictionary<string, object>();
            foreach (var svcSet in App.Config.Content.ContentProviders[0].MediaServicesSets)
            {
                foreach (var svcAcct in svcSet.MediaServicesAccounts)
                {
                    try
                    {
                        svc = new AzureMediaService(svcAcct.AccountName, svcAcct.AccountKey);
                        MonitoringWorker mw = new MonitoringWorker(svc, svcSet.DataStorageConnections);
                        mw.Run();
                    }
                    catch (Exception ex)
                    {
                        if(!errorList.ContainsKey(ex.Message))
                            errorList.Add(ex.Message, ex);               
                    }
                }
                   
            }
            Assert.IsTrue((errorList.Count == 0), "Media Service Account issues found!\r\nPlease use Debug Test to determine cause");
        }
    }
}
