using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;

namespace MediaDashboard.Persistence.Storage
{
    public partial class AMSDashboardEntities1 : DbContext
    {
        public AMSDashboardEntities1(string connStr) : base(connStr) { }
        public AMSDashboardEntities1(EntityConnection conn) : base(conn, true) { }
    }

}
