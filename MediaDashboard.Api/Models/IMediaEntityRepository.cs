using System;
using System.Collections.Generic;

namespace MediaDashboard.Api.Models
{
    interface IMediaEntityRepository
    {
        IEnumerable<Object> GetAll();
        Object Get(string id);
    }
}
