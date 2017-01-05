using Newtonsoft.Json;
using System.Collections.Generic;
using MediaDashboard.Common.Helpers;

namespace MediaDashboard.Common.Data
{
    public class AventusPackager : IAventusObject
    {

        [JsonProperty("Publishers")]
        public List<AventusPublisher> Publishers { get; set; }

        [JsonProperty("AggregateBitRate")]
        public long AggregateBitRate { get; set; }

        [JsonProperty("LastFragmentDuration")]
        public long LastFragmentDuration { get; set; }

        [JsonProperty("TotalAudioStreams")]
        public int TotalAudioStreams { get; set; }

        [JsonProperty("TotalVideoStreams")]
        public int TotalVideoStreams { get; set; }

        [JsonProperty("TotalAudioLanguages")]
        public int TotalAudioLanguages { get; set; }

        AventusHealth _health;
        [JsonProperty("Health")]
        public AventusHealth Health { get
            {
                return _health;
            }
            set { _health = GetMax(Publishers); }
        }

        [JsonProperty("HealthLevel")]
        public HealthStatus HealthLevel
        {
            get
            {
                return Health.HealthLevel.GetHealthStatus();
            }
        }

        private AventusHealth GetMax(List<AventusPublisher> publishers)
        {
            AventusHealth result = null;
            if (publishers.Count > 0)
            {
                int lvl = 0;
                int thisLevel = 0;
                
                foreach(var pub in publishers)
                {
                    switch (pub.Health.HealthLevel.ToLower())
                    {
                        case "normal":
                            thisLevel = 1;
                            if (thisLevel > lvl)
                            {
                                result = pub.Health;
                            }
                            break;
                        case "critical":
                            thisLevel = 3;
                            if (thisLevel > lvl)
                            {
                                result = pub.Health;
                            }
                            break;
                        case "warning":
                            thisLevel = 2;
                            if (thisLevel > lvl)
                            {
                                result = pub.Health;
                            }
                            break;
                        default:
                            break;
                    }
                }
                //return result;
            }
            else
                result = new AventusHealth
                {
                    HealthLevel = "none"
                };
            return result;
        }

        public AventusPackager()
        {
            Publishers = new List<AventusPublisher>();
        }
    }
}
