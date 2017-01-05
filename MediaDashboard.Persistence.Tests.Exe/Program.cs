using System;
using MediaDashboard.Persistence.Caching;

namespace MediaDashboard.Persistence.Tests.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string key = "Key1" + Guid.NewGuid().ToString("D");
                const string value = "Value1";

                MdCache.CacheRole = "enterprisemedia.cache.windows.net";
                MdCache.AzureAlways = true;

                IMdCache cache = MdCache.Instance;

                cache.Set(key, value);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION: " + e);
            }

            Console.WriteLine("Done. Press any key");
            Console.Read();
        }
    }
}
