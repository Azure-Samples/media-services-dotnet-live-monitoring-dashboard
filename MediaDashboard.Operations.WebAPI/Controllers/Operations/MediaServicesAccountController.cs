using MediaDashboard.Common;
using MediaDashboard.Persistence.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Controllers.Operations
{
    public class MediaServicesAccountController : ControllerBase
    {
        // GET: api/MediaServicesAccount
       [HttpGet]
       [Route("api/MediaServicesAccount")]
        public override IEnumerable<object> Get()
        {
            return DataAccess.GetMediaServicesAccounts()
                .ToList <MediaServicesAccount>();
        }
        
        // GET: api/Account/5
        [HttpGet]
        [Route("api/MediaServicesAccount/{id}")]
        public override object Get(string acctName)
        {
            return DataAccess.GetMediaServiceAccount(acctName) as MediaServicesAccount;
        }

        // POST: api/Account
        [HttpPost]
        public void Post([FromBody]string value)
        {
            MediaServicesAccount acct = new MediaServicesAccount();

            /* I know tha tthe string coming in is going to be a json string, but from there, I'm lost*/
        }

        // PUT: api/Account/5
        //[HttpPut]
        //public override IEnumerable<object> Put(int id, [FromBody]string value)
        //{
        //    return new List<MediaServicesAccount>();
        //}
        [HttpPut]
        public override object Put(string id, [FromBody]string value)
        {
            return DataAccess.SaveAccount(new MediaServicesAccount(), App.Config.Content.ContentProviders[0].MediaServicesSets[0].DataStorageConnections[0].AzureServer).Where(ac=>ac.Name==id).FirstOrDefault(); 
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }

        public override IEnumerable<object> Put(object objToSave)
        {
            return DataAccess.SaveAccount(new MediaServicesAccount(), App.Config.Content.ContentProviders[0].MediaServicesSets[0].DataStorageConnections[0].AzureServer);
        }

        public override object Create(string id, string value)
        {
            throw new NotImplementedException();
        }

        public override object Create(object objToSave)
        {
            throw new NotImplementedException();
        }
    }
}
