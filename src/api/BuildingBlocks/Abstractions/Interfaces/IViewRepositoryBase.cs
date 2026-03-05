using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace WePlayRises.BuildingBlocks.Abstractions.Interfaces
{
    public interface IViewRepositoryBase<TEntity>
      where TEntity : class
    {
        /// <summary>
        /// Obtiene una entidad que coincide con la expresión especificada.
        /// </summary>
        /// <param name="where">Expresión para filtrar la entidad.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>La entidad encontrada o null si no se encuentra.</returns>
        TEntity Get(Expression<Func<TEntity, bool>> where);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAllWithTracking();

        TEntity GetById(int id);

        TEntity GetById(string id);

        TEntity GetById<TKey>(TKey id);

        IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// Obtiene una entidad que coincide con la expresión especificada de forma asíncrona.
        /// </summary>
        /// <param name="where">Expresión para filtrar la entidad.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>La entidad encontrada o null si no se encuentra.</returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene una entidad por su identificador (int).
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>La entidad encontrada o null si no se encuentra.</returns>
        Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene una entidad por su identificador (string).
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>La entidad encontrada o null si no se encuentra.</returns>
        Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene una entidad por su identificador genérico (para strongly-typed IDs).
        /// </summary>
        /// <typeparam name="TKey">Tipo del identificador.</typeparam>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>La entidad encontrada o null si no se encuentra.</returns>
        Task<TEntity> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);

    }
}
