using System;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace MediaDashboard.Persistence.Caching.Internal.Azure
{
    internal class ContentionRetryStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception e)
        {
            return e is ContentionIssueException;
        }
    }
}
