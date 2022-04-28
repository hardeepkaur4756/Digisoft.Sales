using Digisoft.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Digisoft.Sales.Service.Interface
{
    public interface IClientService : IService<Client>
    {
        ClientViewModel GetByIDVM(int id);
        Client InsertUpdate(ClientViewModel clientVM);
    }
}