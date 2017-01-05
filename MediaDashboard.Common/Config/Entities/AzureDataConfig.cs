using Newtonsoft.Json;


namespace MediaDashboard.Common.Config.Entities
{
    public class AzureDataConfig
    {
        [JsonProperty("accountName")]
        public string AcctName { get; set; }

        [JsonProperty("azureServer")]
        public string AzureServer { get; set; }

        [JsonProperty("initialCatalog")]
        public string InitialCatalog { get; set; }

        [JsonProperty("dbUserName")]
        public string UserName { get; set; }

        [JsonProperty("dbPassword")]
        public string Password { get; set; }

        public string BasicConnectionString
        {
            get
            {
                return string.Format(@"Data Source={0};Integrated Security=False;User ID={1};Password={2};Initial Catalog={3};Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;", AzureServer, UserName, Password, InitialCatalog);
            }
        }
    }
}
