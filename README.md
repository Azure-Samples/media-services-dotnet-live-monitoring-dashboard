---
services: media-services
platforms: javascript
author: krishndu
---

# Media Services: Live Monitoring Dashboard
Azure Media Services Live Monitoring Dashboard for tracking the health of media services

## Information
The Live Monitoring Dashboard enables Azure Media Services (AMS) customers to view the health of their channel and origin deployments. The dashboard captures the state of ingest, archive, encode, and origin telemetry entities. The dashboard supplies data on the incoming data rate for video stream ingestion, dropped data in storage archive, encoding data rate, and origin HTTP error statuses and latencies. The functionality and usage of the Live Monitoring Dashboard is detailed in our [blog post](https://azure.microsoft.com/en-au/blog/azure-media-services-live-monitoring-dashboard-open-source-release/).

### Preview of the Dashboard![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/dashboard.PNG)

### Special Thanks
Special thanks to [Prakash Duggaraju](https://github.com/duggaraju) for his help and contributions to this project.

## Getting Started
### Create Azure Active Directory (AAD) Tenant
To deploy this application, first create a host Azure Active Directory tenant through the [old Azure portal](manage.windowsazure.com/). In this tenant, create an application by clicking on **Add** under the **Applications** tab. Next, provide an application name and properties, as illustrated below.
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-applications.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-create-application.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-application-properties.png)
### Update Application Manifest
Next, update the application manifest to specify the type of roles supported by the application. Once the application is created, click on **Manage Manifest** then **Download Manifest**. In the downloaded manifest, replace the **appRoles** line with the content of the **rolesettings.xml** file from the **\Monitoring Dashboard\MediaDashboard** directory. Upload the updated manifest file. Lastly, generate and save a tenant client secret key. Make sure to record this key in a safe place.
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-application-created.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/original-manifest.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/updated-manifest.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-upload-manifest.png)
### Update Application Configuration File
Next, in the [new Azure portal](portal.azure.com/), create a web application and SQL database. Update the **contentProviders** section of the template configuration file, **mediadashboardconfig.json**, with parameters for your media service account, associated storage account, SQL database. Upload this configuration file into a blob storage container named **config** in your associated storage account.
Next, add the following key-value pairs to your application's properties (make sure to fill in your associated storage account's name and key for the MediaDashboard.ConfigurationFileAccount):

 - MediaDashboard.ConfigurationFileAccount - DefaultEndpointsProtocol=https;AccountName=**storageaccountname**;AccountKey=**storageaccountkey**
 -  ida:ClientId - The client ID of the AAD app
 -  ida:ClientSecret - The client secret generated in the AAD app
 -  ida:TenantId - The tenant ID of the AAD app

The image below illustrates where to source these values from.
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-client-properties.PNG)
### Upload Storage Account Configuration File
Next, filled out the capitalized field entries in the **mediadashboardconfig.json** file. The fields to complete are as follows (optional fields denoted in parentheses):

 - contentProviders.name: descriptive name of the content provider (optional)
 - contentProviders.mediaServicesSet.descriptiveName: descriptive name of the azure media account
 - contentProviders.mediaServicesSet.dataStorageConnections.accountName: server account name (optional)
 - contentProviders.mediaServicesSet.dataStorageConnections.azureServer: server name, typically of the form: server.database.windows.net
 - contentProviders.mediaServicesSet.dataStorageConnections.initialCatalog: initial catalog name (optional)
 - contentProviders.mediaServicesSet.dataStorageConnections.dbUserName: server database username
 - contentProviders.mediaServicesSet.dataStorageConnections.dbPassword: server database password
 - contentProviders.mediaServicesAccounts.id: media account ID
 - contentProviders.mediaServicesAccounts.accountName: media account name
 - contentProviders.mediaServicesAccounts.accountKey: media account key
 - contentProviders.mediaServicesAccounts.metaData.location: media account location (optional)
 - contentProviders.telemetryStorage.accountName: telemetry storage account name
 - contentProviders.telemetryStorage.accountKey: telemetry storage account key
 
Next, upload **mediadashboardconfig.json** to a blob named **config** in your storage account. This can be done using the [Microsoft Azure Storage Explorer](http://storageexplorer.com/).
### Update Web Configuration
Next, update the template **Web.config** file in the **MediaDashboard.Web** project in Visual Studio. The fields to compelte are as follows:

 - ida:ClientId: client ID of the Azure Active Directory app
 - ida:Domain: domain of the Azure Active Directory app
 - ida:TenantId: Tenant ID from Azure Active Directory app

### Assign User Roles
Lastly, specify the user role for all individuals who will access the application as either **Operator** or **Administrator** under the **User** tab of the application page.
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-user-role-assignments.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-assign-user-roles.png)
### Run the SQL preparation script
Open the solution in Visual Studio, and select the SQL script which is in \Other\Create AMS DB.sql. Double-click on it to open and select **Execute** (icon on the top left). This opens a dialog box where you need to select the database in Azure that you previously created. Authenticate to connect to the database and run the script to prepare it.
### Deploy from Visual Studio
The last step is to build and deploy the application from Visual Studio. Open the solution in Visual Studio, right-click on the **MediaDashboard.Web** project, select **Publish** and complete the Publish Web Application wizard. Create a new publish profile and select **Microsoft Azure Web Apps** as your publish target. Enter the credentials for your Azure subscription and select the web app that you created. Update the Connection string with the domain name of your Azure site. Alternatively, you can download the Publish Profile for your application for the Azure Portal and upload it through the Publish wizard in Visual Studio.

On the Settings page, deselect **Enable Organizational Authentication** (this will recreate the application and overwrite the configuration from the previous steps). Lastly, publish the app to the destination.
#### Debugging Errors
If your receive any errors after publishing the application, you can debug by launching the Log Stream for your application from the Azure Portal. Under the **Development tools** section, launch **Advanced tools**. Next, select **Log stream** from the **Tools** menu. This stream can be refreshed each time you restart or republish the app for debugging purposes.
