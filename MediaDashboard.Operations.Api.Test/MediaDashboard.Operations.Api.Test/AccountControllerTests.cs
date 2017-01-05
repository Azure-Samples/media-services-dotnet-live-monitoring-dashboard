using System;
using System.Diagnostics;
using System.Linq;
using MediaDashboard.Common;
using MediaDashboard.Web.Api.Controllers;
using NUnit.Framework;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Test
{
    [TestFixture]
    public class AccountControllerTests
    {
        [Test]
        public void GetAccountsFromControllerTest()
        {
            var AccountGroupController = new CustomerGroupsController();

            var accts = AccountGroupController.Get();
            Assert.IsTrue(accts != null);
            Assert.IsTrue(accts.Count() >= 0);
        }

        [Test]
        public void GetAccountByNameTest()
        {
            var customerController = new CustomersController();
            var customerConfig = App.Config.Content.ContentProviders[0].MediaServicesSets[0];

            var acct = customerController.Get(customerConfig.Name);
            Assert.IsNotNull(acct, string.Format("Account named \"{0}\" not found", customerConfig.Name));
            Assert.IsTrue(acct.ChannelCount >= 0);
            Assert.IsTrue(acct.Health >= 0);

        }

         [Test]
        public void GetAccountDetailsTest()
        {
            var customerController = new CustomersController();
            string acctName = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0].AccountName;

            try
            {
                var acctDetails = customerController.Get(acctName);
                Assert.IsTrue(acctDetails.ChannelCount >= 0);
                Assert.IsTrue(acctDetails.Health >= 0);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        [Test]
        public void GetNonExistentAccountByNameTest()
        {
            var AccountController = new CustomersController();
            string acctName = "MyTestAccount";

            try
            {
                AccountController.Get(acctName);
            }
            catch(HttpResponseException he)
            {
                return;
            }
            Assert.Fail("Exception expected");
           
        }
    }
}
