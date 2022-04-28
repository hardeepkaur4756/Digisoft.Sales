using System.Collections.Generic;

namespace Digisoft.Sales.Repositories.Interface
{
    internal interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetbyId(int id);
        T Insert(T entity);
        T Update(T entity);
        void Delete(int id);
    }
}
