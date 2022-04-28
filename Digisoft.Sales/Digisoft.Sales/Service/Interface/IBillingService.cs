using Digisoft.Sales.Models;
using Digisoft.Sales.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Service.Interface
{
    public interface IBillingService : IService<Billing>
    { 
        BillingViewModel GetByIDVM(int id);
        Billing InsertUpdate(BillingViewModel billingVM);
        int GetBillingCount(ControllerTypeEnum.ControllerType checkFor, int id);
    }
}