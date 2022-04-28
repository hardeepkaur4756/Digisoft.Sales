using Digisoft.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Service.Interface
{
    public interface IJobService : IService<Job>
    {
        JobViewModel GetByIDVM(int id);
        Job InsertUpdate(JobViewModel clientVM);
    }
}