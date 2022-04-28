using Digisoft.Sales.Models;
using System.Collections.Generic;

namespace Digisoft.Sales.Service.Interface
{
    public interface IBiddingService : IService<Bidding>
    {
        //void Delete(int id);
        //IEnumerable<Bidding> GetAll();
        //Bidding GetByID(int id);
        AddEditBiddingViewModel GetByIDVM(int id);
        Bidding InsertUpdate(AddEditBiddingViewModel biddingVM);
    }
}
