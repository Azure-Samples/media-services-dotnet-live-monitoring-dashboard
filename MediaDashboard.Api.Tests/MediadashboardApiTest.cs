using MediaDashboard.Api.Data;
using MediaDashboard.Api.Mappers;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Persistence.Caching;
using NUnit.Framework;
using System.Collections.Generic;

namespace MediaDashboard.Api.Tests
{
    public class MediaDashboardApiTest
    {
        [TestFixture]
        public class ApiUnitTests
        {
            [Test]
            public void ContentConfigToInformationTest1()
            {
                ContentConfig config = new ContentConfig
                {
                    DashboardTitle = "Title",
                    ContentProviders = new List<ContentProviderConfig>
                {
                    new ContentProviderConfig
                    {
                        Id = "id1"
                    }
                }
                };

                Information info = ContentConfigToInformation.Map(config);

                Assert.NotNull(info);
                Assert.AreEqual(1, info.ContentProviders.Count);
                Assert.AreEqual(config.ContentProviders[0].Id, info.ContentProviders[0].Id);
            }

            [Test]
            public void ContentConfigToInformationTest2()
            {
                const string msaId = "msa-id-1";
                ContentConfig config = new ContentConfig
                {
                    DashboardTitle = "Title",
                    ContentProviders = new List<ContentProviderConfig>
                {
                    new ContentProviderConfig
                    {
                        Id = "id1",
                        MediaServicesSets = new List<MediaServicesSetConfig>
                        {
                            new MediaServicesSetConfig
                            {
                                MediaServicesAccounts = new List<MediaServicesAccountConfig>
                                {
                                    new MediaServicesAccountConfig
                                    {
                                        Id = msaId,
                                        MetaData = new MediaServicesMetaDataConfig
                                        {
                                            AzureSubscriptionId = "azuresubid",
                                            Location = "Amsterdam"
                                        }
                                    }
                                },
                                DataStorageConnections=new List<AzureDataConfig>
                                {
                                    new AzureDataConfig
                                    {
                                        AcctName="accoutName1",
                                        AzureServer="azure-db1-server-name",
                                        InitialCatalog="azure-db1-name",
                                        UserName="azure-db1-userName",
                                        Password="azure-db1-password"
                                    },
                                    new AzureDataConfig
                                    {
                                        AcctName="accoutName2",
                                        AzureServer="azure-db2-server-name",
                                        InitialCatalog="azure-db2-name",
                                        UserName="azure-db2-userName",
                                        Password="azure-db2-password"
                                    }
                                }
                            }
                        }                        
                    }
                }
                };

                MediaService mediaService = new MediaService
                {
                    Id = msaId,
                    Name = "Name1"
                };

                MdCache.Instance.SetAs(MediaService.GetCacheKey(msaId), mediaService);

                Information info = ContentConfigToInformation.Map(config);

                Assert.NotNull(info);
                Assert.AreEqual(1, info.ContentProviders.Count);
                Assert.AreEqual(1, info.ContentProviders[0].MediaPipelines.Count);
                Assert.AreEqual(1, info.ContentProviders[0].MediaPipelines[0].Deployments.Count);
                Assert.AreEqual(msaId, info.ContentProviders[0].MediaPipelines[0].Deployments[0].Id);

                List<AzureDataConfig> dbConns = config.ContentProviders[0].MediaServicesSets[0].DataStorageConnections;
                Assert.IsNotNull(dbConns);
                Assert.IsNotEmpty(dbConns);
                Assert.AreEqual(2, dbConns.Count);
                Assert.AreNotSame(dbConns[0], dbConns[1]);

            }
        }

    }
}
