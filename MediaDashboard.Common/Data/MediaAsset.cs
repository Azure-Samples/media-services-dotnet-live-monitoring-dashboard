using System;
using System.Collections.Generic;

namespace MediaDashboard.Common.Data
{
    public class MediaAsset
    {
        string AlternateId { get; set; }
        List<string> AssetFiles { get; set; }
        List<string> ContentKeys { get; set; }
        DateTime Created { get; set; }
        string Id { get; set; }
        DateTime LastModified { get; set; }
        List<MediaLocator> Locators { get; set; }
        string Name { get; set; }
        object Options { get; set; }
        object ParentAssets { get; set; }
        string State { get; set; }
        object StorageAccount { get; set; }
        object StorageAccountName { get; set; }
        string Uri { get; set; }
    }
}
