using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class MediaService
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Datacenter { get; set; }
        public HealthStatus Health { get; set; }

        public string CacheKey
        {
            get { return GetCacheKey(Id); }
        }

        public static string GetCacheKey(string serviceId)
        {
            return string.Format("MediaService-{0}", serviceId);
        }

        public List<MediaChannel> Channels;

        public List<MediaProgram> Programs;

        public List<MediaOrigin> Origins;

        // List of origins used for VOD only.
        public List<string> VodOrigins;
    }
}
