using System.IO;
using System.Text;
using Microsoft.ApplicationServer.Caching;

namespace MediaDashboard.Persistence.Caching.Internal.Azure
{
    internal class AzureMdCacheSerializer : IDataCacheObjectSerializer
    {
        public void Serialize(Stream stream, object value)
        {
            byte[] raw = Encoding.UTF8.GetBytes(value.ToString());
            stream.Write(raw, 0, raw.Length);
        }

        public object Deserialize(Stream stream)
        {
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
