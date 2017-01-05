using System;
using System.IO;
using System.Text;
using MediaDashboard.Common;
using System.Diagnostics;

namespace MediaDashboard.Persistence.Caching.Internal.Local
{
    public class LocalMdCache : BaseMdCache
    {

        private string _cacheDir;
        public LocalMdCache()
        {
            _cacheDir = Environment.ExpandEnvironmentVariables(App.Config.Sys.Cache.LocalCacheDir);
            Trace.TraceInformation("Cache directory Initialized to {0}", _cacheDir);
        }

        public override string Get(string key)
        {
            Validate.NotNull(key, "key");

            string filePath = GetFileName(key);
            try
            {
                if (File.Exists(filePath))
                    return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed to get key:{0} from cache path:{1}! Error:{2}", key, filePath, ex);
            }
            return null;
        }

        public override void Set(string key, string value)
        {
            Validate.NotNull(key, "key");

            string filePath = GetFileName(key);
            try
            {
                Directory.CreateDirectory(new FileInfo(filePath).DirectoryName);
                File.WriteAllText(filePath, value, Encoding.UTF8);
            }
            catch(Exception ex)
            {
                Trace.TraceWarning("Failed to add key:{0} to cache path:{1}! Error:{2}", key, filePath, ex);
            }
        }

        public override void Set(string key, string value, TimeSpan lifeSpan)
        {
            Set(key, value);
        }

        private string GetFileName(string key)
        {
            // Since key can contain ':' escape it with - to avoid file system errors.
            return Path.Combine(_cacheDir, key.Replace(':', '-'));
        }
    }
}