using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDashboard.Persistence.Storage
{
    public partial class AMSDashboardEntities1 : DbContext
    {
        public AMSDashboardEntities1(string connStr) : base(connStr) { }
        public AMSDashboardEntities1(EntityConnection conn) : base(conn, true) { }
    }

}
