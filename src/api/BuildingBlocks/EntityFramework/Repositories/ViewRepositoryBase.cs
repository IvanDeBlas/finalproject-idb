using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.Abstractions.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.BuildingBlocks.EntityFramework.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace WePlayRises.BuildingBlocks.EntityFramework.Repositories
{
    public abstract class ViewRepositoryBase<TEntity> : IViewRepositoryBase<TEntity>, IDisposable
     where TEntity : class
    {
        private readonly CoreDbContext _mainContext;
        private bool _disposed;

        protected CoreDbContext MainContext => _mainContext ?? throw new ObjectDisposedException(nameof(MainContext));

        protected ViewRepositoryBase(CoreDbContext context)
        {
            _mainContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Public Methods
        public TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                return _mainContext.Set<TEntity>().Where(where).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException("Error when trying to get by expression asynchronously", ex);
            }
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            try
            {
                return _mainContext.Set<TEntity>().AsNoTracking().AsQueryable();
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException("Error when trying to get all", ex);
            }
        }

        public virtual IQueryable<TEntity> GetAllWithTracking()
        {
            try
            {
                return _mainContext.Set<TEntity>().AsQueryable();
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException("Error when trying to get all with tracking", ex);
            }
        }

        public virtual TEntity GetById(int id)
        {
            try
            {
                return _mainContext.Set<TEntity>().Find(id);
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException($"Error when trying to get by id (int): {id}", ex);
            }
        }

        [ExcludeFromCodeCoverage]
        public virtual TEntity GetById(string id)
        {
            try
            {
                return _mainContext.Set<TEntity>().Find(id);
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException($"Error when trying to get by id (string): {id}", ex);
            }
        }

        public virtual TEntity GetById<TKey>(TKey id)
        {
            try
            {
                return _mainContext.Set<TEntity>().Find(id);
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException($"Error when trying to get by id ({typeof(TKey).Name}): {id}", ex);
            }
        }

        public virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                return _mainContext.Set<TEntity>().AsNoTracking().Where(where).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException("Error when trying to get many", ex);
            }
        }
        #endregion Public Methods

        #region Public Async Methods
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _mainContext.Set<TEntity>().Where(where).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException("Error when trying to get by expression asynchronously", ex);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _mainContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException($"Error when trying to get by id (int): {id}", ex);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _mainContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException($"Error when trying to get by id (string): {id}", ex);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _mainContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new ViewRepositoryException($"Error when trying to get by id ({typeof(TKey).Name}): {id}", ex);
            }
        }
        #endregion Public Async Methods

        #region Dispose Pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _mainContext?.Dispose();
            }

            _disposed = true;
        }
        #endregion Dispose Pattern
    }

}
