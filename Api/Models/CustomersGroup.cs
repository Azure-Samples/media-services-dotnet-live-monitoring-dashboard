using System.Collections.Generic;

namespace MediaDashboard.Operations.Api.Models
{
    public class CustomersGroup
    {
        public string Name { get; set; }
        public List<Customer> Customers { get; set; }
    }
}
