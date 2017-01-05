using System;
using System.Collections.Generic;
using MediaDashboard.Common.Data;

namespace MediaDashboard.Api.Models
{
    public class MediaChannelsRepo:IMediaEntityRepository
    {
        private List<MediaChannel> myInternalentityList = new List<MediaChannel>();
        public MediaChannelsRepo()
        {
            var cache = MediaDashboard.Persistence.Caching.MdCache.Instance;

            //TODO: read from Cache
            for (int i = 0; i < 10; i++)
            {
                myInternalentityList.Add(new MediaChannel() { Id = i.ToString(), LastModified = DateTime.Now });
            }
        }
        public IEnumerable<object> GetAll()
        {
            return myInternalentityList;
        }

        public object Get(string id)
        {
            return myInternalentityList.Find(x => x.Id == id);
        }
    }

   
}