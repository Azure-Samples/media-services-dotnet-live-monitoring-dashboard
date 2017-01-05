using System;
using System.Collections.Generic;
using MediaDashboard.Common.Config.Entities;

namespace MediaDashboard.Common.Config.ConfigReaders
{
    //TODO: Move file and implement pattern to ingest Config Reader instead of this
    public class SampleConfigReader : IConfigReader
    {
        public MediaDashboardConfig ReadConfig()
        {
            var conf = new MediaDashboardConfig
            {
                Content = new ContentConfig
                {
                    DashboardTitle = "Dashboard-title-goes-here",
                    ContentProviders = new List<ContentProviderConfig>
                    {
                        new ContentProviderConfig
                        {
                            Id = "CP-id-1",
                            Name = "Viasat",
                            MediaServicesSets = new List<MediaServicesSetConfig>
                            {
                                new MediaServicesSetConfig
                                {
                                    Name = "Sample Simple Mediaservices Setup",
                                    MediaServicesAccounts = new List<MediaServicesAccountConfig>
                                    {
                                        new MediaServicesAccountConfig
                                        {
                                            Id = string.Format("id1"),
                                            AccountName = "test1",
                                            AccountKey = "secret-key-goes-here",
                                            AdminUri = new Uri("http://test1.mediaservices.windows.net"),
                                            MetaData = new MediaServicesMetaDataConfig
                                            {
                                                AzureSubscriptionId = "yyyy-yyyy-yyyy-yyyy",
                                                Location = "Amsterdam"
                                            }
                                        }
                                    },
                                    LoggingAccounts = new List<StorageAccountConfig>
                                    {
                                        new StorageAccountConfig
                                        {
                                            AccountName = "account-name-goes-here",
                                            AccountKey = "account-key-goes-here"
                                        }
                                    }
                                },
                                new MediaServicesSetConfig
                                {
                                    Name = "Sample High Availability Mediaservices Setup",
                                    MediaServicesAccounts = new List<MediaServicesAccountConfig>
                                    {
                                        new MediaServicesAccountConfig
                                        {
                                            Id = string.Format("id2"),
                                            AccountName = "test2",
                                            AccountKey = "secret-key-goes-here",
                                            AdminUri = new Uri("http://test2.mediaservices.windows.net"),
                                            MetaData = new MediaServicesMetaDataConfig
                                            {
                                                AzureSubscriptionId = "yyyy-yyyy-yyyy-yyyy",
                                                Location = "Amsterdam"
                                            }
                                        },
                                        new MediaServicesAccountConfig
                                        {
                                            Id = string.Format("id3"),
                                            AccountName = "test3",
                                            AccountKey = "secret-key-goes-here",
                                            AdminUri = new Uri("http://test3.mediaservices.windows.net"),
                                            MetaData = new MediaServicesMetaDataConfig
                                            {
                                                AzureSubscriptionId = "qqqq-qqqq-qqqq-qqqq",
                                                Location = "Dublin"
                                            }
                                        },
                                    },
                                    LoggingAccounts = new List<StorageAccountConfig>
                                    {
                                        new StorageAccountConfig
                                        {
                                            AccountName = "account-name1-goes-here",
                                            AccountKey = "account-key1-goes-here"
                                        },
                                        new StorageAccountConfig
                                        {
                                            AccountName = "account-name2-goes-here",
                                            AccountKey = "account-key2-goes-here"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Sys = new SysConfig
                {
                    Cache = new CacheConfig
                    {
                        AuthorizationToken = "dfd",
                        CacheName = "funkyName",
                        Identifier = "funkyIdentifier"
                    },
                    Maintenance = new MaintenanceConfig
                    {
                        SqlRentionPeriod = 10,
                        TableStorageRetentionPeriod = 30
                    }
                }
            };
            return conf;
        }
    }
}