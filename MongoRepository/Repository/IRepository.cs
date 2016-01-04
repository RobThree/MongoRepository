﻿namespace MongoRepository
{
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// IRepository definition.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public interface IRepository<T, TKey> : IQueryable<T>
        where T : IEntity<TKey>
    {
        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations).
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
        /// for most purposes you can use the MongoRepositoryManager&lt;T&gt;
        /// </remarks>
        /// <value>The Mongo collection (to perform advanced operations).</value>
        IMongoCollection<T> Collection { get; }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        T GetById(TKey id);

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        T Add(T entity);

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        void Add(IEnumerable<T> entities);

        /// <summary>
        /// Adds the new entity in the repository asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Adds the new entities in the repository asynchronously.
        /// <param name="entities">The entities to add.</param>
        /// </summary>
        Task AddAsync(IEnumerable<T> entities);

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        T Update(T entity);

        /// <summary>
        /// Upserts an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// Upserts the entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        Task UpdateAsync(IEnumerable<T> entities);


        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        void Delete(TKey id);

        /// <summary>
        /// Deletes an entity from the repository by its ObjectId asynchronously.
        /// </summary>
        /// <param name="id">The ObjectId of the entity.</param>
        void DeleteAsync(TKey id);

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(T entity);

        /// <summary>
        /// Deletes the given entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes the entities matching the predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        Task DeleteAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Deletes all entities in the repository asynchronously.
        /// </summary>
        void DeleteAllAsync();

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the repository.</returns>
        long Count();

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        bool Exists(Expression<Func<T, bool>> predicate);
    }

    /// <summary>
    /// IRepository definition.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public interface IRepository<T> : IQueryable<T>, IRepository<T, string>
        where T : IEntity<string> { }
}
