namespace MongoRepository
{
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public interface IRepositoryAsync<T, TKey> : IRepository<T, TKey>
        where T : IEntity<TKey>
    {
        Task<T> GetByIdAsync(TKey id);

        /// <summary>
        /// Returns all of the entities in the repository.
        /// </summary>
        /// <returns>A List of type T containing all of the entities in the repository.</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        Task AddAsync(IEnumerable<T> entities);

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        Task UpdateAsync(IEnumerable<T> entities);

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        Task<DeleteResult> DeleteAsync(TKey Id);
        
        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        Task<DeleteResult> DeleteAsync(T entity);

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        Task<DeleteResult> DeleteAllAsync();

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the repository.</returns>
        Task<long> CountAsync();

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    }

    /// <summary>
    /// IRepository definition.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public interface IRepositoryAsync<T> : IRepositoryAsync<T, string>
        where T : IEntity<string>
    { }
}
