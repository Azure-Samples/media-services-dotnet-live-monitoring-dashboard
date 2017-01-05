using Microsoft.WindowsAzure.MediaServices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class MediaOrigin
    {
        static private int ReservedUnitCapacity = 160;

        [JsonProperty("Health")]
        public HealthStatus Health { get; set; }

        [JsonProperty("Created")]
        public DateTime Created { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        public string HostName { get; set; }

        public string Id { get; set; }

        [JsonProperty("LastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        //Short name to display in seating chart.
        public string NameShort { get; set; }

        public int ReservedUnits { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        /// <summary>
        /// Calculated values
        /// </summary>
        public int Capacity
        {
            get { return (this.ReservedUnits * ReservedUnitCapacity); }
        }

        public int Throughput   // Bytes
        {
            get { return 83886080; }
        }
        
        public int Utilization
        {
            get
            {
                decimal temp = (Capacity == 0) ? 0: ((decimal)Throughput / (decimal)((Capacity * 1024)*1024));

                return (int)(temp * 100);
            }
        }

        public DateTime LastMetricUpdate;

        public List<IMetricBase> Metrics;

        public string IPAllowList { get; set; }

        public List<AkamaiSignatureHeaderAuthenticationKey> AuthenticationKeys { get; set; }

        public TimeSpan? MaxAge { get; set; }
    }
}
