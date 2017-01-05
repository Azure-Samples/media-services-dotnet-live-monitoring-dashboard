using System;
using Microsoft.ApplicationServer.Caching;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace MediaDashboard.Persistence.Caching.Internal.Azure
{
    internal static class ContentionRetryPolicy
    {
        private static RetryPolicy Default { get; set; }

        static ContentionRetryPolicy()
        {
            // just approximation
            Default = new RetryPolicy(new ContentionRetryStrategy(), 600, TimeSpan.FromMilliseconds(5));
        }

        public static void ExecuteAction(Action action)
        {
            Default.ExecuteAction(() => InvokeAction(action));
        }

        private static void InvokeAction(Action action)
        {
            try
            {
                action();
            }
            catch (DataCacheException e)
            {
                switch (e.ErrorCode)
                {
                    case DataCacheErrorCode.Timeout:
                    case DataCacheErrorCode.RetryLater:
                        throw new ContentionIssueException();
                    default:
                        throw;
                }
            }
        }
    }
}
