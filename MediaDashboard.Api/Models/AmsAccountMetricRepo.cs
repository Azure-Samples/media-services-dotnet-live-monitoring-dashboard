using MediaDashboard.Common.Data;
using MediaDashboard.Persistence.Caching;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MediaDashboard.Api.Models
{
    public class AmsAccountMetricRepo
    {
        private IMdCache _cache = MdCache.Instance;
        private MediaService amsMetricInfo;
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Channels")]
        public List<MediaChannel> Channels {get {return amsMetricInfo.Channels;}}
        [JsonProperty("Programs")]
        public List<MediaProgram> Programs { get { return amsMetricInfo.Programs; } }
        [JsonProperty("Origins")]
        public List<MediaOrigin> Origins { get { return amsMetricInfo.Origins; } }
        private string CacheKey
        {
            get { return string.Format("MediaService-{0}", this.Id); }
        }
        public AmsAccountMetricRepo(string AccountId)
        {
           
            Id = AccountId;
            UpdateState();
            Name = amsMetricInfo.Name;
        
        }
        public void UpdateState()
        {
            //read from cache

            amsMetricInfo = _cache.GetAs<MediaService>(this.CacheKey);
        }
        public MediaChannel GetChannelMetricDatail(string Id)
        {
            return this.amsMetricInfo.Channels.Find(X => X.Id == Id);
        }
       
    }
}