using System.Linq.Expressions;
using Base.Contracts.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Base.Contracts.Persistence
{
    /// <summary>
    /// Generic repository interface for accessing and manipulating entities.
    /// Provides common data access operations for entities implementing <see cref="IEntityObject"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class, IEntityObject, new()
    {
        /// <summary>
        /// Returns a queryable collection of entities with optional filtering, ordering, and eager loading.
        /// </summary>
        /// <param name="disableTracking">If true, disables change tracking for better performance.</param>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="orderBy">Optional ordering function.</param>
        /// <param name="includeProperties">Optional list of related navigation properties to include.</param>
        /// <returns>A queryable collection of entities.</returns>
        IQueryable<TEntity> GetAsync(
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties
        );

        /// <summary>
        /// Returns a paginated list of entities with optional filtering, ordering, and eager loading.
        /// </summary>
        /// <param name="page">The zero-based page index.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="disableTracking">If true, disables change tracking.</param>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="orderBy">Optional ordering function.</param>
        /// <param name="includeProperties">Optional navigation properties to include.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of entities.</returns>
        Task<List<TEntity>> GetPageAsync(
            int page,
            int pageSize,
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// Returns a queryable collection of projected results with optional filtering, ordering, and eager loading.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="disableTracking">If true, disables change tracking.</param>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="orderBy">Optional ordering function.</param>
        /// <param name="selector">Projection expression that defines the output shape.</param>
        /// <param name="includeProperties">Optional navigation properties to include.</param>
        /// <returns>A queryable collection of projected results.</returns>
        IQueryable<TResult> GetProjected<TResult>(
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, TResult>>? selector = null,
            params Expression<Func<TEntity, object>>[] includeProperties
        );

        /// <summary>
        /// Returns a paginated list of projected results with optional filtering, ordering, and eager loading.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="page">The zero-based page index.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="disableTracking">If true, disables change tracking.</param>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="orderBy">Optional ordering function.</param>
        /// <param name="selector">Projection expression that defines the output shape.</param>
        /// <param name="includeProperties">Optional navigation properties to include.</param>
        /// <returns>A task representing the asynchronous operation. The result contains a list of projected results.</returns>
        Task<List<TResult>> GetProjectedPageAsync<TResult>(
            int page,
            int pageSize,
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, TResult>>? selector = null,
            params Expression<Func<TEntity, object>>[] includeProperties
        );

        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the entity, or null if not found.</returns>
        Task<TEntity?> GetByIdAsync(int id);

        /// <summary>
        /// Checks whether an entity with the given primary key exists.
        /// </summary>
        /// <param name="id">The primary key of the entity.</param>
        /// <returns>A task representing the asynchronous operation. The task result indicates whether the entity exists.</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Adds a new entity to the data store.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that returns the EntityEntry of the added entity.</returns>
        Task<EntityEntry<TEntity>> AddAsync(TEntity entity);

        /// <summary>
        /// Adds a collection of entities to the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Attaches the given entity to the DbContext and marks it as modified.
        /// </summary>
        /// <param name="entity">The entity to attach and mark as modified.</param>
        void Attach(TEntity entity);

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to delete.</param>
        /// <returns>A task that returns true if the entity was found and deleted, otherwise false.</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Removes the specified entity from the data store.
        /// </summary>
        /// <param name="entityToDelete">The entity to remove.</param>
        void Remove(TEntity entityToDelete);

        /// <summary>
        /// Updates the specified entity in the data store.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Returns the count of entities that match the given filter.
        /// </summary>
        /// <param name="filter">Optional filter expression to count matching entities.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the count of matching entities.</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);

        /// <summary>
        /// Checks whether there are any changes being tracked in the DbContext.
        /// </summary>
        /// <returns>True if changes are being tracked; otherwise, false.</returns>
        bool HasChanges();
    }
}
