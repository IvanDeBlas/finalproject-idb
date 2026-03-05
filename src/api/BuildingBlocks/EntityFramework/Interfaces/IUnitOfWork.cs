using WePlayRises.BuildingBlocks.EntityFramework.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WePlayRises.BuildingBlocks.EntityFramework.Interfaces
{
    public interface IUnitOfWork<T> : IDisposable
            where T : CoreDbContext
    {
        int Commit();

        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        void ResetCount();
    }
}
