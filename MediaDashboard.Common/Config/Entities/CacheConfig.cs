using Newtonsoft.Json;

namespace MediaDashboard.Common.Config.Entities
{
    public class CacheConfig
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("cacheName")]
        public string CacheName { get; set; }

        [JsonProperty("authorizationToken")]
        public string AuthorizationToken { get; set; }

        [JsonProperty("localCacheDir")]
        public string LocalCacheDir { get; set; }
    }
}