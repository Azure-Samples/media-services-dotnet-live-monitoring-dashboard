using System;
using MediaDashboard.Common;
using Microsoft.ApplicationServer.Caching;

namespace MediaDashboard.Persistence.Caching.Internal.Azure
{
    public class AzureMdCache : BaseMdCache
    {
        private readonly MdCacheConfig _config;
        private volatile DataCache _dataCache;

        /// <summary>
        /// Do not use this constructor. Use MdCache.Instance instead.
        /// </summary>
        public AzureMdCache(MdCacheConfig config)
        {
            Validate.NotNull(config, "config");

            _config = config;

            DataCacheFactoryConfiguration factoryConfig = new DataCacheFactoryConfiguration()
            {
                SerializationProperties = new DataCacheSerializationProperties(
                    DataCacheObjectSerializerType.CustomSerializer,
                    new AzureMdCacheSerializer()
                    ),
                IsCompressionEnabled = true,
                AutoDiscoverProperty = new DataCacheAutoDiscoverProperty(true, config.Identifier)
            };

            if (!string.IsNullOrEmpty(config.AuthorizationToken))
                factoryConfig.SecurityProperties = new DataCacheSecurity(config.AuthorizationToken);
            
            DataCacheFactory factory = new DataCacheFactory(factoryConfig);

            _dataCache = factory.GetCache(config.CacheName);
        }

        public override void Set(string key, string value)
        {
            Validate.NotNull(key, "key");

            Execute(cache => cache.Put(key, value));
        }

        public override void Set(string key, string value, TimeSpan lifeSpan)
        {
            Validate.NotNull(key, "key");

            Execute(cache => cache.Put(key, value, lifeSpan));
        }

        public override string Get(string key)
        {
            string value = null;

            Execute(cache =>
            {
                value = cache.Get(key) as string;
            });

            return value;
        }

        private void Execute(Action<DataCache> action)
        {
            ContentionRetryPolicy.ExecuteAction(() => action(_dataCache));
        }
    }
}
