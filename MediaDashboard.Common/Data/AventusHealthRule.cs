using Newtonsoft.Json;
using System;

namespace MediaDashboard.Common.Data
{
    public class AventusHealthRule : IComparable
    {

        [JsonProperty("RuleId")]
        public string RuleID { get; set; }

        [JsonProperty("HealthLevel")]
        public string HealthLevel { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }

        public int CompareTo(object obj)
        {
            int result = 0;
            AventusHealthRule healthObj = obj as AventusHealthRule;
            if (healthObj != null)
            {
                switch (healthObj.HealthLevel)
                {
                    case "Critical":
                        result = 10;
                        break;
                    case "Warning":
                        result = 5;
                        break;
                    case "Normal":
                        result = 1;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

    }
}
