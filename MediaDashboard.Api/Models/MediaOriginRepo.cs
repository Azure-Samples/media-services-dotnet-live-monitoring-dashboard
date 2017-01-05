using MediaDashboard.Common.Data;
using System.Collections.Generic;

namespace MediaDashboard.Api.Models
{
    public class MediaOriginRepo:IMediaEntityRepository
    {
        private List<MediaOrigin> myInternalentityList = new List<MediaOrigin>();
         public MediaOriginRepo()
        {
            //TODO: read from Cache
            for (int i = 0; i < 10; i++)
            {
                //myInternalentityList.Add(new MediaOrigin() { Id = i.ToString(), LastModified = DateTime.Now });
            }
        }
        public IEnumerable<object> GetAll()
        {
            return myInternalentityList;
        }

        public object Get(string id)
        {
            return null;// myInternalentityList.Find(x => x.Id == id);
        }
    }
}