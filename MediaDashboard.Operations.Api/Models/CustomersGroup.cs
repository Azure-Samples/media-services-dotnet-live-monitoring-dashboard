using System.Collections.Generic;

namespace MediaDashboard.Web.Api.Models
{
    public class CustomersGroup
    {
        public string Name { get; set; }
        public List<Customer> Customers { get; set; }
    }
}
