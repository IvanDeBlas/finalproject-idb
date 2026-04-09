using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.Abstractions.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.BuildingBlocks.EntityFramework.Exceptions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace WePlayRises.BuildingBlocks.EntityFramework.Repositories
{
    public abstract class RepositoryBase<TEntity> : ViewRepositoryBase<TEntity>, IRepositoryBase<TEntity>
         where TEntity : class
    {
        public new CoreDbContext MainContext { get; set; }


        protected RepositoryBase(CoreDbContext context) : base(context)
        {
            MainContext = context;
        }

        #region Public Methods
        public virtual void Add(TEntity entityToAdd)
        {
            try
            {
                MainContext.Add(entityToAdd);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to add entity", ex);
            }
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            try
            {
                MainContext.Remove(entityToDelete);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to delete entity", ex);
            }
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                var objects = MainContext.Set<TEntity>().Where(where).AsEnumerable();
                foreach (var obj in objects)
                    MainContext.Remove(obj);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to delete expression", ex);
            }
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            try
            {
                var entry = MainContext.Entry(entityToUpdate);
                if (entry.State == EntityState.Detached)
                {
                    // Check if there's already a tracked entity with the same key
                    var keyValues = MainContext.Model.FindEntityType(typeof(TEntity))?
                        .FindPrimaryKey()?
                        .Properties
                        .Select(p => entry.Property(p.Name).CurrentValue)
                        .ToArray();

                    if (keyValues != null)
                    {
                        var trackedEntity = MainContext.Set<TEntity>().Local
                            .FirstOrDefault(e =>
                            {
                                var trackedEntry = MainContext.Entry(e);
                                var trackedKeyValues = MainContext.Model.FindEntityType(typeof(TEntity))?
                                    .FindPrimaryKey()?
                                    .Properties
                                    .Select(p => trackedEntry.Property(p.Name).CurrentValue)
                                    .ToArray();

                                return keyValues.SequenceEqual(trackedKeyValues ?? Array.Empty<object>());
                            });

                        if (trackedEntity != null)
                        {
                            // Copy only non-null/non-default values from the new entity to the tracked entity
                            var trackedEntry = MainContext.Entry(trackedEntity);
                            var newEntry = MainContext.Entry(entityToUpdate);

                            foreach (var property in newEntry.Properties)
                            {
                                // Skip navigation properties and primary keys
                                if (property.Metadata.IsPrimaryKey())
                                    continue;

                                var newValue = property.CurrentValue;
                                var propertyType = property.Metadata.ClrType;

                                // Only copy non-null reference types and non-default value types
                                if (newValue != null)
                                {
                                    var defaultValue = propertyType.IsValueType
                                        ? Activator.CreateInstance(propertyType)
                                        : null;

                                    // For strings, only copy if not null (empty string is valid)
                                    // For value types, only copy if not default
                                    if (propertyType == typeof(string) || !Equals(newValue, defaultValue))
                                    {
                                        trackedEntry.Property(property.Metadata.Name).CurrentValue = newValue;
                                    }
                                }
                            }
                            return;
                        }
                    }

                    MainContext.Attach(entityToUpdate);
                }
                MainContext.Entry(entityToUpdate).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to update entity", ex);
            }
        }
        #endregion Public Methods

        #region Public Async Methods
        public virtual async Task AddAsync(TEntity entityToAdd, CancellationToken cancellationToken = default)
        {
            try
            {
                await MainContext.AddAsync(entityToAdd, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to add entity", ex);
            }
        }

        public virtual async Task DeleteAsync(TEntity entityToDelete, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() => MainContext.Remove(entityToDelete), cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to delete entity", ex);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() =>
                {
                    var objects = MainContext.Set<TEntity>().Where(where).AsEnumerable();
                    foreach (var obj in objects)
                        MainContext.Remove(obj);
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to delete expression", ex);
            }
        }

        public virtual async Task UpdateAsync(TEntity entityToUpdate, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() =>
                {
                    var entry = MainContext.Entry(entityToUpdate);
                    if (entry.State == EntityState.Detached)
                    {
                        // Check if there's already a tracked entity with the same key
                        var keyValues = MainContext.Model.FindEntityType(typeof(TEntity))?
                            .FindPrimaryKey()?
                            .Properties
                            .Select(p => entry.Property(p.Name).CurrentValue)
                            .ToArray();

                        if (keyValues != null)
                        {
                            var trackedEntity = MainContext.Set<TEntity>().Local
                                .FirstOrDefault(e =>
                                {
                                    var trackedEntry = MainContext.Entry(e);
                                    var trackedKeyValues = MainContext.Model.FindEntityType(typeof(TEntity))?
                                        .FindPrimaryKey()?
                                        .Properties
                                        .Select(p => trackedEntry.Property(p.Name).CurrentValue)
                                        .ToArray();

                                    return keyValues.SequenceEqual(trackedKeyValues ?? Array.Empty<object>());
                                });

                            if (trackedEntity != null)
                            {
                                // Copy values from the new entity to the tracked entity
                                MainContext.Entry(trackedEntity).CurrentValues.SetValues(entityToUpdate);
                                return;
                            }
                        }

                        MainContext.Attach(entityToUpdate);
                    }
                    MainContext.Entry(entityToUpdate).State = EntityState.Modified;
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error when trying to update entity", ex);
            }
        }
        #endregion Public Async Methods
    }
}
