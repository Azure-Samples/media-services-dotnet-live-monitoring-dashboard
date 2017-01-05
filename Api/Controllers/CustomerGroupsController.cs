using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Operations.Api.Models;
using MediaDashboard.Persistence.Storage;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sdk = Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaDashboard.Operations.Api.Controllers
{
    [Route("api/[controller]")]
    public class CustomerGroupsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<CustomersGroup> Get()
        {
            var groups = App.Config.Content.ContentProviders.Select(contentProvider => new 
            {
                CustomersGroup = new CustomersGroup {  Name = contentProvider.Name},
                ContentProvider = contentProvider
            }).ToList();

            Parallel.ForEach(groups, (customersGroup) =>
            {
                GetCustomersGroup(customersGroup.ContentProvider, customersGroup.CustomersGroup);
            });
            return groups.Select(group => group.CustomersGroup);
        }

        private void GetCustomersGroup(ContentProviderConfig contentProvider, CustomersGroup customersGroup)
        {
            customersGroup.Customers = contentProvider.MediaServicesSets.Select(mediaServicesSet => new Customer
            {
                Id = mediaServicesSet.Name,// TODO is there an actual ID value we can use instead of the name of hte account for the Customer ID?
                Name = mediaServicesSet.Name
            }).ToList();

            Parallel.ForEach(customersGroup.Customers, (customer) =>
            {
                var mediaServicesSet = contentProvider.MediaServicesSets.First(m => m.Name == customer.Name);
                GetCustomer(customer, mediaServicesSet);
            });
        }

        private void FillCustomerDetails(Customer customer, MediaServicesAccountConfig account)
        {
            var mediaServicesContext = account.GetContext();

            var programs = mediaServicesContext.Programs.ToList();
            customer.ProgramCount = programs.Count();
            customer.ChannelCount = mediaServicesContext.Channels.Count();
            customer.OriginCount = mediaServicesContext.StreamingEndpoints.Count();
            customer.ArchiveCount = programs.Count(
                program => program.State == Microsoft.WindowsAzure.MediaServices.Client.ProgramState.Running);
        }

        private void GetCustomer(Customer customer, MediaServicesSetConfig mediaServicesSet)
        {
            var dataAccess = new AzureDataAccess(mediaServicesSet.DataStorageConnections);
            customer.Accounts = mediaServicesSet.MediaServicesAccounts.Select((mediaServicesAccount, index) =>
            {
                var account = GetMediaAccountFromCache(customer, mediaServicesAccount);
                if(account == null)
                {
                    if (index == 0)
                    {
                        FillCustomerDetails(customer, mediaServicesAccount);
                    }
                    // TODO: this logic is wrong and needs to be replaced.
                    var globalAerts = dataAccess.GetAccoutGlobalAlerts(mediaServicesAccount.AccountName);
                    return new MediaService
                    {
                        Id = mediaServicesAccount.Id,
                        Name = mediaServicesAccount.AccountName,
                        Datacenter = mediaServicesAccount.MetaData.Location,
                        Health = GetMaxLevel(globalAerts),
                    };
                }
                return account;
            }).ToList();
            customer.Health = customer.Accounts.Max(m => m.Health);
        }

        private MediaService GetMediaAccountFromCache(Customer customer, MediaServicesAccountConfig accountConfig)
        {
            var account = CloudCache.GetAs<MediaService>(accountConfig.Id);
            if (account != null)
            {
                customer.ChannelCount = account.Channels.Count;
                customer.OriginCount = account.Origins.Count;
                customer.ProgramCount = account.Programs.Count;
                customer.ArchiveCount = account.Programs.Where(p => p.State == Sdk.ProgramState.Running.ToString()).Count();
                return new MediaService
                {
                    Id = account.Id,
                    Name = account.Name,
                    Datacenter = account.Datacenter,
                    Health = account.Health
                };
            }
            return null;
        }
    }
}
