using MediaDashboard.Common.Data;
using System.Collections.Generic;

namespace MediaDashboard.Api.Models
{
    public class MediaProgramRepo : IMediaEntityRepository
    {
        private List<MediaProgram> myInternalentityList = new List<MediaProgram>();
        public MediaProgramRepo()
        {
            //TODO: read from Cache
            for (int i = 0; i < 10; i++)
            {
                 // myInternalentityList.Add(new MediaProgramRepo() { });
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