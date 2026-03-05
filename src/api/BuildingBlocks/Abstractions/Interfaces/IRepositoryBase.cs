using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace WePlayRises.BuildingBlocks.Abstractions.Interfaces
{
    public interface IRepositoryBase<TEntity> : IViewRepositoryBase<TEntity>
            where TEntity : class
    {
        #region Public Methods
        /// <summary>
        /// Agrega una entidad al contexto.
        /// </summary>
        /// <param name="entityToAdd">La entidad que se desea agregar.</param>
        void Add(TEntity entityToAdd);

        /// <summary>
        /// Elimina una entidad del contexto.
        /// </summary>
        /// <param name="entityToDelete">La entidad que se desea eliminar.</param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Elimina entidades que coincidan con una expresión especificada.
        /// </summary>
        /// <param name="where">Expresión para filtrar las entidades a eliminar.</param>
        void Delete(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// Actualiza una entidad en el contexto.
        /// </summary>
        /// <param name="entityToUpdate">La entidad que se desea actualizar.</param>
        void Update(TEntity entityToUpdate);
        #endregion Public Methods


        #region Public Async Methods

        /// <summary>
        /// Agrega una entidad al contexto de forma asíncrona.
        /// </summary>
        /// <param name="entityToAdd">La entidad que se desea agregar.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación.</param>
        Task AddAsync(TEntity entityToAdd, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina una entidad del contexto de forma asíncrona.
        /// </summary>
        /// <param name="entityToDelete">La entidad que se desea eliminar.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación.</param>
        Task DeleteAsync(TEntity entityToDelete, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina entidades que coincidan con una expresión especificada de forma asíncrona.
        /// </summary>
        /// <param name="where">Expresión para filtrar las entidades a eliminar.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación.</param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza una entidad en el contexto de forma asíncrona.
        /// </summary>
        /// <param name="entityToUpdate">La entidad que se desea actualizar.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación.</param>
        Task UpdateAsync(TEntity entityToUpdate, CancellationToken cancellationToken = default);

        #endregion Public Async Methods
    }
}
