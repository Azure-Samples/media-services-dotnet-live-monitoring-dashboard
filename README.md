---
services: media-services
platforms: javascript
author: krishndu
---

# Media Services: Live Monitoring Dashboard
Azure Media Services Live Monitoring Dashboard for tracking the health of media services

## Information
The Live Monitoring Dashboard enables Azure Media Services (AMS) customers to view the health of their channel and origin deployments. The dashboard captures the state of ingest, archive, encode, and origin telemetry entities. The dashboard supplies data on the incoming data rate for video stream ingestion, dropped data in storage archive, encoding data rate, and origin HTTP error statuses and latencies.

### Preview of the Dashboard
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/dashboard.PNG)

### Special Thanks
Special thanks to [Prakash Duggaraju](https://github.com/duggaraju) for his help and contributions to this project.

## Getting Started
### Create Azure Active Directory (AAD) Tenant
To deploy this application, first create a host Azure Active Directory tenant through the [old Azure portal](manage.windowsazure.com/). In this tenant, create an application by clicking on **Add** under the **Applications** tab. Next, provide an application name and properties, as illustrated below.
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-applications.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-create-application.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-application-properties.png)
### Update Application Manifest
Next, update the application manifest to specify the type of roles supported by the application. Once the application is created, click on **Manage Manifest** then **Download Manifest**. In the downloaded manifest, replace the **appRoles** line with the content of the **rolesettiings.xml** file from the **\Monitoring Dashboard\MediaDashboard** directory. Upload the updated manifest file. Lastly, generate and save a tenant client secret key. Make sure to record this key in a safe place.
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
### Assign User Roles
Lastly, specify the user role for all individuals who will access the application as either **Operator** or **Administrator** under the **User** tab of the application page.
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-user-role-assignments.png)
![Dashboard Sample](https://github.com/Azure-Samples/media-services-dotnet-live-monitoring-dashboard/raw/master/Documentation/azure-manage-portal-assign-user-roles.png)
### Deploy from Visual Studio
The last step is to build and deploy the application from Visual Studio. Open the solution in Visual Studio, right-click on the **MediaDashboard.Web** project, select **Publish** and complete the Publish Web Application wizard. Create a new publish profile and select **Microsoft Azure Web Apps** as your publish target. Enter the credentials for your Azure subscription and select the web app that you created. Update the Connection string with the domain name of your Azure site. On the Settings page, unselect **Enable Organizational Authentication** (this will recreate the application and overwrite the configuration from the previous steps). Lastly, publish the app to the destination.
