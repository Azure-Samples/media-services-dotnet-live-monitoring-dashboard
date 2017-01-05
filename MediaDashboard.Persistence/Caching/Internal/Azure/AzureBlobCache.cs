using MediaDashboard.Common.Config.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MediaDashboard.Persistence.Caching.Internal.Azure
{
    /**
    * A class that uses table storage as cache.
    */
    public class AzureBlobCache : BaseMdCache
    {

        private const string CacheContainerName = "mediadashboardcache";

        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;

        public AzureBlobCache(StorageAccountConfig storageConfig):
            this(new CloudStorageAccount(new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey), true))
        {
        }

        public AzureBlobCache(string connectionString):
            this(CloudStorageAccount.Parse(connectionString))
        {
        }

        private AzureBlobCache(CloudStorageAccount account)
        {
            Trace.TraceInformation("Azure blob cache initialized with storage account:{0}", account.Credentials.AccountName);
            _blobClient = account.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference(CacheContainerName);
            _container.CreateIfNotExists();
        }


        public override string Get(string key)
        {
            var blob = _container.GetBlockBlobReference(key);
            try
            {
                if (!blob.Exists())
                    return null;

                DateTimeOffset dtLastModified = blob.Properties.LastModified.Value;
                DateTimeOffset dtUtcNow = DateTime.UtcNow;

                if (dtLastModified.AddMinutes(2) < dtUtcNow) // cache is invalidated as blob is older than 2 minutes
                {
                    Trace.TraceWarning("Deleting stale cache entry  for key:{0} to blob:{1}! last updated: {2} - {3} = {4}", key, blob.Uri, dtLastModified , dtUtcNow, dtUtcNow - dtLastModified);
                    blob.DeleteAsync();
                    return null;
                }

                using (var stream = new MemoryStream((int)blob.Properties.Length))
                {
                    blob.DownloadToStream(stream);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed to read key:{0} to blob:{1}! Error:{2}", key, blob.Uri, ex);
            }
            return null;
        }

        public override void Set(string key, string value)
        {
            var blob = _container.GetBlockBlobReference(key);
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    blob.UploadFromStream(stream);
                }
            }
            catch(Exception ex)
            {
                Trace.TraceWarning("Failed to add key:{0} to blob:{1}! Error:{2}", key, blob.Uri, ex);
            }
        }

        public override void Set(string key, string value, TimeSpan lifeSpan)
        {
            throw new NotImplementedException();
        }

        public string SetAs<T>(string key, T value)
        {
            string serialized = JsonConvert.SerializeObject(value, Formatting.None);
            Set(key, serialized);
            return serialized;
        }

    }
}
