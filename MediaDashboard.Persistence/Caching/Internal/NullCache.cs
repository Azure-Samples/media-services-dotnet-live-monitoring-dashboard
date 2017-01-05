using System;

namespace MediaDashboard.Persistence.Caching.Internal
{
    /*
    A Null cache that doesn't cache anything. 
    */
    class NullCache : BaseMdCache
    {
        public override string Get(string key)
        {
            return null;
        }

        public override void Set(string key, string value)
        {            
        }

        public override void Set(string key, string value, TimeSpan lifeSpan)
        {
        }
    }
}
