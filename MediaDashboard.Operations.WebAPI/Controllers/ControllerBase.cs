using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;

using MediaDashboard.Common;
using MediaDashboard.Common.Config;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Persistence;
using System.Web.Http;
using MediaDashboard.Persistence.Storage;
using Microsoft.WindowsAzure.MediaServices.Client;



namespace MediaDashboard.Operations.Api.Controllers
{
    public abstract class ControllerBase : ApiController
    {
        AzureDataAccess _dataAccess;

        public AzureDataAccess DataAccess
        {
            get { return _dataAccess; }
            set { _dataAccess = value; }
        }
        CloudMediaContext _cloudContext;

        public CloudMediaContext MSCloudContext
        {
            get { return _cloudContext; }
            set { _cloudContext = value; }
        }

        private string _mediaServiceName = string.Empty;

        protected string MediaServiceName
        {
            get { return _mediaServiceName; }
            set { _mediaServiceName = value; }
        }
        private string _mediaServiceKey = string.Empty;

        protected string MediaServiceKey
        {
            get { return _mediaServiceKey; }
            set { _mediaServiceKey = value; }
        }
        public ControllerBase()
        {
            /* This is what pulls the configuration information (including info for the data connections) from the .json config file.
             * I placed it here becasue each controller is going to need it's own connection to the database(s) that it responds to, 
             * AND I am not changing the current schema at this point in time.
             */
            DataAccess = new AzureDataAccess(App.Config.Content.ContentProviders[0].MediaServicesSets[0].DataStorageConnections);

            MediaServiceName = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0].AccountName;
            MediaServiceKey = App.Config.Content.ContentProviders[0].MediaServicesSets[0].MediaServicesAccounts[0].AccountKey;

            var mediaServiceCreds = new MediaServicesCredentials(_mediaServiceName, _mediaServiceKey);
            
            MSCloudContext = new CloudMediaContext(mediaServiceCreds);
            //CloudContext.
        }

        
        [HttpGet]
        public abstract IEnumerable<object> Get();
        [HttpGet]
        public abstract object Get(string objId);

        [HttpPut]
        public abstract IEnumerable<object> Put(object objToSave);

        [HttpPut]
        public abstract object Put(string id, string value);

        [HttpPost]
        public abstract object Create(string id, string value);

        [HttpPost]
        public abstract object Create(object objToSave);
        
    }
}