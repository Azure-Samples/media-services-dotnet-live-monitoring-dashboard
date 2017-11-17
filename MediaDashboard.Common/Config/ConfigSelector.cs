using System;
using System.IO;
using MediaDashboard.Common.Config.ConfigReaders;
using Newtonsoft.Json;

namespace MediaDashboard.Common.Config
{
    internal static class ConfigSelector
    {
        private static IConfigReader _configReader;
        
        public static IConfigReader Get()
        {
            if (_configReader != null)
                return _configReader;

            // If config reader is not set, select new one and return it
            _configReader = SelectConfigReader();
            return _configReader;
        }

        private static IConfigReader SelectConfigReader()
        {
            if (UseCustomConfigOverride())
            {
                return SetupCustomConfigReader();
            }
            
            return SetupStandardConfigReader();
        }

        private static IConfigReader SetupStandardConfigReader()
        {
            // Use standard configuration from .csconfig-files if we are running
            // in Azure or the Emulator and no custom config has been specified
            return new BlobConfigReader(
                MasterConfig.ConfigStorageConnectionString,
                MasterConfig.ConfigContainerName,
                MasterConfig.ConfigBlobName);
        }

        private static IConfigReader SetupCustomConfigReader()
        {
            var customConfig = GetCustomConfigOverrideConfiguration();
            if(customConfig.ConfigType==null)
                throw new Exception("Configuration Type not properly read from .json Configuration");

            switch (customConfig.ConfigType.ToLower())
            {
                case"file":
                    if (!string.IsNullOrWhiteSpace(customConfig.FilePath))
                        return new FileConfigReader(customConfig.FilePath);

                    throw new Exception(
                        "Configuration was set to be read from file, but no file path specified in FilePathAzure or FilePath");

                case "blob":

                    if (!string.IsNullOrWhiteSpace(customConfig.BlobName))
                        return new BlobConfigReader(
                            customConfig.BlobConnectionString,
                            customConfig.BlobContainer,
                            customConfig.BlobName);

                    throw new Exception("Configuration was set to be read from Blob, but not enough parameters where set");
                default:
                    return new FileConfigReader(customConfig.FilePath);
            }
        }

        private static bool UseCustomConfigOverride()
        {
            return File.Exists(GetCustomConfigOverrideFilePath());
        }

        private static string GetCustomConfigOverrideFilePath()
        {
            const string customConfigOverrideFileName = "mediadashboard-override-config.json";
            var myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return Path.Combine(myDocumentsFolder, customConfigOverrideFileName);
        }

        private static CustomConfigOverrideConfig GetCustomConfigOverrideConfiguration()
        {
            var jsonConfig = File.ReadAllText(GetCustomConfigOverrideFilePath());
            return JsonConvert.DeserializeObject<CustomConfigOverrideConfig>(jsonConfig);
        }
    }
}