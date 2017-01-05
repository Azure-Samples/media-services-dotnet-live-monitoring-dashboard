using MediaDashboard.Common;

namespace MediaDashboard.Persistence.Caching
{
    public class MdCacheConfig
    {
        // Either Cache Role name within cloud service or Managed Cache identifier
        public string Identifier { get; private set; }

        public string CacheName { get; private set; }
        
        // Managed Cache authorization key
        public string AuthorizationToken { get; private set; }

        public MdCacheConfig(
            string identifier,
            string cacheName,
            string authorizationToken = null
            )
        {
            Validate.NotNullOrEmpty(identifier, "identifier");
            Validate.NotNullOrEmpty(cacheName, "cacheName");

            Identifier = identifier;
            CacheName = cacheName;
            AuthorizationToken = authorizationToken;
        }
    }
}