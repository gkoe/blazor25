using Base.Contracts.Entities;
using Base.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Base.Persistence
{
    /// <summary>
    /// Generic repository implementation providing common data access logic for any entity.
    /// Specialized repositories can inherit from this class and override methods as needed.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being handled.</typeparam>
    public class GenericRepository<TEntity>(DbContext context, ILogger<GenericRepository<TEntity>> logger)
        : IGenericRepository<TEntity> where TEntity : class, IEntityObject, new()
    {
        // Reference to the DbSet for the entity type
        private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>()
            ?? throw new NullReferenceException("_dbSet is null");

        // Expose the context if needed externally
        public DbContext Context { get; } = context;

        private readonly ILogger<GenericRepository<TEntity>> _logger = logger;


        /// <summary>
        /// Builds a LINQ query with optional filtering, ordering, and eager loading.
        /// </summary>
        private IQueryable<TEntity> BuildQuery(
            bool disableTracking,
            Expression<Func<TEntity, bool>>? filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
            Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            // Disable change tracking to improve performance for read-only operations
            if (disableTracking)
                query = query.AsNoTracking();

            // Apply eager loading for related entities
            if (includeProperties?.Length > 0)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            // Apply optional filter
            if (filter != null)
                query = query.Where(filter);

            // Apply optional sorting
            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        /// <summary>
        /// Retrieves all entities that match optional filters, sorting, and includes.
        /// </summary>
        /// <param name="disableTracking">If true, disables Entity Framework change tracking (improves read performance).</param>
        /// <param name="filter">Optional filter expression (e.g. e => e.IsActive).</param>
        /// <param name="orderBy">Optional sort logic (e.g. q => q.OrderBy(e => e.Name)).</param>
        /// <param name="includeProperties">Navigation properties to include (eager loading).</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a queryable of entities.</returns>
        public IQueryable<TEntity> GetAsync(
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return BuildQuery(disableTracking, filter, orderBy, includeProperties);
        }

        /// <summary>
        /// Retrieves a specific page of entities with optional filtering, sorting, and eager loading.
        /// </summary>
        /// <param name="page">Zero-based page index (0 = first page).</param>
        /// <param name="pageSize">Number of entities per page.</param>
        /// <param name="disableTracking">If true, disables change tracking.</param>
        /// <param name="filter">Optional filter condition.</param>
        /// <param name="orderBy">Optional ordering logic.</param>
        /// <param name="includeProperties">Related entities to include via eager loading.</param>
        /// <returns>A list of entities for the requested page.</returns>
        public virtual async Task<List<TEntity>> GetPageAsync(
            int page,
            int pageSize,
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than or equal to zero.");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            var query = BuildQuery(disableTracking, filter, orderBy, includeProperties);

            return await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves projected entities using a selector expression, with optional filtering, sorting, and includes.
        /// </summary>
        /// <typeparam name="TResult">The type to project the result to.</typeparam>
        /// <param name="disableTracking">Whether to disable EF Core change tracking.</param>
        /// <param name="filter">Filter condition.</param>
        /// <param name="orderBy">Ordering logic.</param>
        /// <param name="selector">Projection logic (e.g. e => new { e.Id, e.Name }).</param>
        /// <param name="includeProperties">Navigation properties to include.</param>
        public virtual IQueryable<TResult> GetProjected<TResult>(
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, TResult>>? selector = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = BuildQuery(disableTracking, filter, orderBy, includeProperties);

            if (selector != null)
                return query.Select(selector);

            if (typeof(TEntity) == typeof(TResult))
                return (IQueryable<TResult>)query;

            throw new InvalidOperationException("No selector provided, and TResult is not the same as TEntity.");
        }

        /// <summary>
        /// Retrieves a paged and projected list of entities.
        /// </summary>
        /// <typeparam name="TResult">The projection result type.</typeparam>
        /// <param name="page">Page number (zero-based).</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="disableTracking">Disable change tracking.</param>
        /// <param name="filter">Optional filter.</param>
        /// <param name="orderBy">Optional ordering.</param>
        /// <param name="selector">Projection logic.</param>
        /// <param name="includeProperties">Navigation properties for eager loading.</param>
        public virtual async Task<List<TResult>> GetProjectedPageAsync<TResult>(
            int page,
            int pageSize,
            bool disableTracking = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, TResult>>? selector = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than or equal to zero.");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            var query = GetProjected(disableTracking, filter, orderBy, selector, includeProperties);

            return await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a single entity by its primary key (Id).
        /// </summary>
        /// <param name="id">Primary key of the entity.</param>
        /// <returns>The entity, or null if not found.</returns>
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    _logger.LogWarning("Entity of type {Entity} with ID {Id} not found.", typeof(TEntity).Name, id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity of type {Entity} with ID {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        /// <summary>
        /// Checks whether an entity with the given Id exists.
        /// </summary>
        /// <param name="id">Primary key of the entity.</param>
        /// <returns>True if entity exists, otherwise false.</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Adds a new entity to the context asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The EntityEntry representing the added entity.</returns>
        public async Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
            try
            {
                var entry = await _dbSet.AddAsync(entity);
                _logger.LogInformation("Entity of type {Entity} added: {@Entity}", typeof(TEntity).Name, entity);
                return entry;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity of type {Entity}: {@Entity}", typeof(TEntity).Name, entity);
                throw;
            }
        }

        /// <summary>
        /// Adds a list of entities to the context in a single batch.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Attaches an entity to the context and marks it as modified.
        /// </summary>
        /// <param name="entity">The entity to attach and mark as modified.</param>
        public void Attach(TEntity entity)
        {
            Context.Attach(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Delete failed: Entity of type {Entity} with ID {Id} not found.", typeof(TEntity).Name, id);
                    return false;
                }

                _dbSet.Remove(entity);
                _logger.LogInformation("Entity of type {Entity} with ID {Id} deleted.", typeof(TEntity).Name, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity of type {Entity} with ID {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        /// <summary>
        /// Removes an entity from the context.
        /// If the entity is not tracked, it is first attached.
        /// </summary>
        /// <param name="entityToRemove">The entity to remove.</param>
        public void Remove(TEntity entityToRemove)
        {
            try
            {
                if (Context.Entry(entityToRemove).State == EntityState.Detached)
                {
                    _dbSet.Attach(entityToRemove);
                }

                _dbSet.Remove(entityToRemove);
                _logger.LogInformation("Entity of type {Entity} removed: {@Entity}", typeof(TEntity).Name, entityToRemove);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing entity of type {Entity}: {@Entity}", typeof(TEntity).Name, entityToRemove);
                throw;
            }
        }

        /// <summary>
        /// Updates an entity by marking it as modified in the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
                _logger.LogInformation("Entity of type {Entity} updated: {@Entity}", typeof(TEntity).Name, entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity of type {Entity}: {@Entity}", typeof(TEntity).Name, entity);
                throw;
            }
        }


        /// <summary>
        /// Counts all entities that match the optional filter.
        /// </summary>
        /// <param name="filter">An optional condition to apply to the count.</param>
        /// <returns>The number of matching entities.</returns>
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter is not null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        /// <summary>
        /// Checks whether there are any pending changes in the context.
        /// </summary>
        /// <returns>True if there are changes, false otherwise.</returns>
        public bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }
    }
}
