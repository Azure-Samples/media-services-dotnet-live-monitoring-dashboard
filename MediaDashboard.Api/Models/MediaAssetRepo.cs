using MediaDashboard.Common.Data;
using System.Collections.Generic;

namespace MediaDashboard.Api.Models
{
    public class MediaAssetRepo:IMediaEntityRepository
    {
         private List<MediaAsset> myInternalentityList = new List<MediaAsset>();
         public MediaAssetRepo()
        {
            //TODO: read from Cache
            for (int i = 0; i < 10; i++)
            {
               // myInternalentityList.Add(new MediaChannel() { Id = i.ToString(), LastModified = DateTime.Now });
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