using System.Collections.Generic;
using System.Threading.Tasks;

namespace WePlayRises.BuildingBlocks.Abstractions.Interfaces
{
    public interface IRepository<TEntityDomain, TEntityDao>
       where TEntityDao : class
       where TEntityDomain : class
    {
        Task Add(string tenant, TEntityDomain obj);
        Task<TEntityDomain> GetById(string tenant, string id);
        Task<IEnumerable<TEntityDomain>> GetAll(string tenant);
        Task Update(string tenant, string id, TEntityDomain obj);
        Task Remove(string tenant, string id);
    }
}
