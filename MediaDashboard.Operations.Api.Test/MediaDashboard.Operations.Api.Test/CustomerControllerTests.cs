using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MediaDashboard.Common;
using MediaDashboard.Operations.Api.Controllers;
using MediaDashboard.Operations.Api.Models;

namespace MediaDashboard.Operations.Api.Test
{
     [TestFixture]
    class CustomerControllerTests
    {
        #region Private methods
        private void CheckStandardComponents(ControllerBase controller)
         {
             Assert.IsNotNull(controller.DataAccess);
             Assert.IsNotNull(controller.MSCloudContext);

         }
        #endregion

        [Test]
         public void GetCustomersGroupsControllerTest()
         {
             var controller = new CustomerGroupsController();
             CheckStandardComponents(controller);

             var cgs = controller.Get();
             Assert.IsNotNull(cgs, "Customer Groups not Poperly defined");
             Assert.IsInstanceOf(typeof(IEnumerable<CustomersGroup>), cgs);
         }

        
         [Test]
         public void GetCustomersControllerTest(){
             var controller = new CustomersController();
             CheckStandardComponents(controller);

         }

         
    }
}
