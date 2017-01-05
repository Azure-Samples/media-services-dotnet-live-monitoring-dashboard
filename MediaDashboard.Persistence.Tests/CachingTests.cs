using System;
using MediaDashboard.Persistence.Caching;
using MediaDashboard.Persistence.Caching.Internal.Azure;
using NUnit.Framework;

namespace MediaDashboard.Persistence.Tests
{
    [TestFixture]
    public class CachingTests
    {
        [Test]
        public void SetGet()
        {
            string key = "Key1" + Guid.NewGuid().ToString("D");
            const string value = "Value1";
            IMdCache cache = MdCache.Instance;

            cache.Set(key, value);

            string readvalue = cache.Get(key);

            Assert.AreEqual(value, readvalue);
        }

        /// <summary>
        /// This test is ignored becasue of the length of time it takes to run
        /// </summary>
        [Test, Ignore("Takes too long to run")]
        public void AzureCacheTest()
        {
            string key = "Key1" + Guid.NewGuid().ToString("D");
            const string value = "Value1";
            
            IMdCache cache = new AzureMdCache(
                new MdCacheConfig(
                    "enterprisemedia.cache.windows.net",
                    "default",
                    "YWNzOmh0dHBzOi8vZW50ZXJwcmlzZW1lZGlhODE3Ni1jYWNoZS5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0Ly9XUkFQdjAuOS8mb3duZXImek1BNXBSR1ZhdXpaZ3ZLM2NGanMrR2N2RGRkWlBVb1pGVDRpNk9HbmlaRT0maHR0cDovL2VudGVycHJpc2VtZWRpYS5jYWNoZS53aW5kb3dzLm5ldC8="
                    )
                );

            cache.Set(key, value, TimeSpan.FromMinutes(1));

            string readvalue = cache.Get(key);

            Assert.AreEqual(value, readvalue);
        }
    }
}
