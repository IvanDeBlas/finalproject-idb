using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Memory.Interfaces;
using WePlayRises.BuildingBlocks.Abstractions.Interfaces;
using WePlayRises.BuildingBlocks.Abstractions.Model;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.BuildingBlocks.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WePlayRises.BuildingBlocks.EntityFramework.Services
{
    public abstract class MasterTableBaseService<TEntity, TException, TContext, TRepository, T>
         where TContext : CoreDbContext
         where TRepository : IRepositoryBase<TEntity>
         where TEntity : MasterTableBase<T>
         where TException : Exception, new()
    {
        private readonly TRepository _repository;
        private readonly IUnitOfWork<TContext> _uow;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        public readonly ILogger _logger;

        private readonly string _cache;

        public MasterTableBaseService(
                                TRepository repository,
                                IUnitOfWork<TContext> uow,
                                IMemoryCacheProvider memoryCacheProvider,
                                ILogger<TEntity> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _memoryCacheProvider = memoryCacheProvider ?? throw new ArgumentNullException(nameof(memoryCacheProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _cache = $"{typeof(TEntity).Name}Cache";
        }

        public List<TEntity> GetAll() =>
            TryGet(() =>
            {
                var itemsCache = _memoryCacheProvider.Get<List<TEntity>>(_cache);
                if (itemsCache == null)
                {
                    _logger.LogInformation($"{typeof(TEntity).Name} cache not found. Getting {typeof(TEntity).Name} from context.");
                    itemsCache = GetAllEntities();

                    _memoryCacheProvider.Set(itemsCache, _cache);
                }

                return itemsCache;
            }, $"Getting all {typeof(TEntity).Name} Cache method called",
            $"Error getting {typeof(TEntity).Name} Cache");

        public TEntity? GetById(T id) =>
            TryGet(() => GetAll().FirstOrDefault(p => p.Id.Equals(id)),
                $"GetById method called, Id: {id}",
                $"Error getting by id: {id}");

        public Task<T> PostAsync(TEntity item, CancellationToken cancellationToken) =>
            TryCatchAsync(async () =>
            {
                await _repository.AddAsync(item, cancellationToken);
                await _uow.CommitAsync(cancellationToken);
                RemoveCache();

                return item.Id;
            }, "PostAsync method called",
            $"Error creating new {typeof(TEntity).Name}",
            cancellationToken);

        public Task<bool> UpdateAsync(TEntity item, CancellationToken cancellationToken) =>
            TryCatchAsync(async () =>
            {
                await _repository.AddAsync(item, cancellationToken);
                var result = await _uow.CommitAsync(cancellationToken);
                RemoveCache();

                return result > 0;
            }, $"Update method called, Id: {item.Id}",
            $"Error updating id: {item.Id}",
            cancellationToken);

        public Task<bool> DeleteAsync(T id, CancellationToken cancellationToken) =>
            TryCatchAsync(async () =>
            {
                var item = GetById(id);
                if (item == null)
                    return false;

                await _repository.AddAsync(item, cancellationToken);
                var result = await _uow.CommitAsync(cancellationToken);
                RemoveCache();

                return result > 0;
            }, $"Delete method called, Id: {id}",
            $"Error deleting id: {id}",
            cancellationToken);

        public (bool exists, T? id) ItemExistsWithName(string name) =>
            TryGet(() =>
            {
                var all = GetAll();
                if (all.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    var item = all.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
                    return (true, item.Id);
                }

                return (false, default(T));
            }, $"ItemExistsWithName method called, name: {name}",
                    $"Error ItemExistsWithName: {name}");

        public bool ItemExists(T id) =>
           TryGet(() => GetAll().Any(p => p.Id.Equals(id)),
               $"{typeof(TEntity).Name} Exists method called, Id: {id}",
               $"Error checking {typeof(TEntity).Name} exists id: {id}");

        private List<TEntity> GetAllEntities() =>
          TryGet(() => _repository.GetAll().ToList(),
              "GetAllEntities method called",
              $"Error getting {typeof(TEntity).Name}");

        private void RemoveCache()
        {
            _memoryCacheProvider.Remove(_cache);
        }


        internal T TryGet<T>(Func<T> func, string logInfo, string errorMessage)
        {
            try
            {
                _logger.LogInformation(logInfo);
                return func();
            }
            catch (Exception ex)
            {
                throw SetException(errorMessage, ex);
            }
        }

        internal async Task<T> TryCatchAsync<T>(Func<Task<T>> func, string logInfo, CancellationToken cancellationToken) =>
            await TryCatchAsync<T>(func, logInfo, null, cancellationToken);

        internal async Task<T> TryCatchAsync<T>(Func<Task<T>> func, string logInfo, string errorMessage, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(logInfo);
                return await Task.Run(() => func(), cancellationToken);
            }
            catch (Exception ex)
            {
                throw SetException(errorMessage ??= logInfo, ex);
            }
        }

        internal abstract TException SetException(string errorMessage, Exception ex);
    }
}
