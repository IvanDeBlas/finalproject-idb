using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.BuildingBlocks.EntityFramework.Exceptions;
using WePlayRises.BuildingBlocks.EntityFramework.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace WePlayRises.BuildingBlocks.EntityFramework.UoW
{
    public class UnitOfWork<T> : IUnitOfWork<T>, IDisposable
     where T : CoreDbContext
    {
        protected T Context { get; private set; }

        public UnitOfWork(T context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int Commit()
        {
            return Context.SaveChanges();
            //return CommitAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            if (Context == null)
                throw new InvalidOperationException("Context has already been disposed.");

            try
            {
                return await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateException dbEx)
            {
                throw new UnitOfWorkException("Database update failed", dbEx);
            }
            catch (Exception ex)
            {
                throw new UnitOfWorkException("Error when trying to save changes asynchronously", ex);
            }
        }

        public void ResetCount()
        {
            Commit(); // Reutiliza el método Commit para evitar duplicar la lógica
        }

        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            Context?.Dispose();
            Context = null;
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
