using System;
using MediaDashboard.Common;
using MediaDashboard.Persistence.Caching.Internal;
using MediaDashboard.Persistence.Caching.Internal.Azure;
using MediaDashboard.Persistence.Caching.Internal.Local;
using MediaDashboard.Common.Config;
using System.Diagnostics;

namespace MediaDashboard.Persistence.Caching
{
    /// <summary>
    /// MdCache - Meda Data Cache
    /// </summary>
    public static class MdCache
    {
        public static MdCacheConfig Config { get; set; }

        public static IMdCache Instance
        {
            get { return Nested._instance; }
        }

        private static class Nested
        {
            static Nested()
            {
                try
                {
                    var cacheConfig = App.Config.Sys.Cache;
                    if (null == Config)
                    {
                        Config = new MdCacheConfig(cacheConfig.Identifier, cacheConfig.CacheName, cacheConfig.AuthorizationToken);
                    }

                    if (MasterConfig.ConfigStorageConnectionString != null)
                    {
                        Trace.TraceInformation("[MdCache] Attaching to BlobCache {0}", MasterConfig.ConfigStorageConnectionString);
                        _instance = new AzureBlobCache(MasterConfig.ConfigStorageConnectionString);
                    }
                    else if (!string.IsNullOrWhiteSpace(cacheConfig.LocalCacheDir))
                    {
                        Trace.TraceInformation("[MdCache] Attaching to LocalCache");
                        _instance = new LocalMdCache();
                    }
                    else if (Config.CacheName != null)
                    {
                        _instance = new AzureMdCache(Config);
                    }
                    else
                    {
                        _instance = new NullCache();
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("[MdCache] Nested.Nested() failed. {0}", e);
                    throw;
                }
            }

            internal readonly static IMdCache _instance;
        }
    }

    
}
