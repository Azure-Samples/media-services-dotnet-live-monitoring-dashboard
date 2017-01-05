using MediaDashboard.Common;
using MediaDashboard.Common.Config.Entities;
using MediaDashboard.Common.Data;
using MediaDashboard.Common.Helpers;
using MediaDashboard.Operations.Api.Models;
using Microsoft.AspNet.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace MediaDashboard.Operations.Api.Controllers
{
    [Route("/api/Customers")]
    public class CustomersController : ControllerBase
    {

        public CustomersController() : base() { }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public Customer Get(string id)
        {
            Stopwatch watch = new Stopwatch();
            var mediaServicesSet = App.Config.GetCustomer(id);
            if( mediaServicesSet == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            Customer customer = new Customer
            {
                Id = mediaServicesSet.Name,
                Name = mediaServicesSet.Name,
                Accounts = mediaServicesSet.MediaServicesAccounts.Select(accountConfig =>
                {
                    var account = CloudCache.GetAs<MediaService>(accountConfig.Id);
                    if (account == null)
                    {
                        Trace.TraceWarning("Cache miss for account:{0}", accountConfig.Id);
                        account = new MediaService
                        {
                            Id = accountConfig.Id,
                            Name = accountConfig.AccountName,
                            Datacenter = accountConfig.MetaData.Location
                        };
                    }
                    return account;
                }).ToList()
            };

            // Get each of the account details in parallel.
            Parallel.ForEach(customer.Accounts.Where(account => account.Channels == null), 
                (account,state,i) => GetAccountDetails(mediaServicesSet.MediaServicesAccounts[(int)i], account));
            customer.Health = customer.Accounts.Select(account => account.Health).DefaultIfEmpty(HealthStatus.Healthy).Max();

            Trace.TraceInformation("Time taken load customer details...: {0}", watch.Elapsed);
            return customer;
        }

        private void GetAccountDetails(MediaServicesAccountConfig accountConfig, MediaService accountInfo)
        {
            var programs = Task.Run(() => GetProgramDetails(accountConfig, accountInfo));
            var channels = Task.Run(() => GetChannelDetails(accountConfig, accountInfo));
            var origins = Task.Run(() => GetOriginDetails(accountConfig, accountInfo));
            Task.WhenAll(channels, origins, programs).Wait();
            accountInfo.FindChannelOriginMapping();
        }

        private static void GetChannelDetails(MediaServicesAccountConfig accountConfig, MediaService accountInfo)
        {
            var channelsController = new ChannelsController();
            accountInfo.Channels = channelsController.GetAllChannels(accountConfig);
            accountInfo.Health = accountInfo.Channels.Max(c => c.Health);
        }

        private void GetOriginDetails(MediaServicesAccountConfig accountConfig, MediaService accountInfo)
        {
            var originsController = new OriginsController();
            accountInfo.Origins = originsController.GetOrigins(accountConfig);
        }

        private void GetProgramDetails(MediaServicesAccountConfig accountConfig, MediaService accountInfo)
        {
            var programsController = new ProgramsController();
            accountInfo.Programs = programsController.GetAllPrograms(accountConfig);
        }
    }
}
