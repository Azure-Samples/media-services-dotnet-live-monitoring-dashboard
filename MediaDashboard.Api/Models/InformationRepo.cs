using MediaDashboard.Api.Data;
using MediaDashboard.Common;
using System;

namespace MediaDashboard.Api.Models
{
    public class InformationRepo : AbstractInformationRepository
    {
        public override object Get()
        {
            if (App.Config != null && App.Config.Content != null)
            {
                var config = App.Config.Content;
                Information information = Mappers.ContentConfigToInformation.Map(config);

                return information;
            }

            throw new Exception();
        }
    }
}