using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DreamSongs.MongoRepository
{
    /// <summary>
    /// IRepository definition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : Entity
    {
        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations)
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class.
        /// </remarks>
        [Obsolete("This property will be removed in future releases.")]
        MongoCollection<T> Collection { get; }

        /// <summary>
        /// Returns the T by its given id
        /// </summary>
        /// <param name="id">The string representing the ObjectId of the entity to retrieve</param>
        /// <returns>The Entity T</returns>
        T GetById(string id);

        /// <summary>
        /// Returns the T by its given ObjectId
        /// </summary>
        /// <param name="id">The string representing the ObjectId of the entity to retrieve</param>
        /// <returns>The Entity T</returns>
        T GetById(ObjectId id);


        /// <summary>
        /// Returns a single T by the given criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        /// <returns>The T</returns>
        T GetSingle(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Returns All the records of T
        /// </summary>
        /// <returns>IQueryable of T</returns>
        IQueryable<T> All();

        /// <summary>
        /// Returns the list of T where it matches the criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        /// <returns>IQueryable of T</returns>
        IQueryable<T> All(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Adds the new entity in the repository
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity including its new ObjectId</returns>
        T Add(T entity);

        /// <summary>
        /// Adds the new entities in the repository
        /// </summary>
        /// <param name="entities">The entities of type T</param>
        void Add(IEnumerable<T> entities);

        /// <summary>
        /// Upserts an entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The updated entity</returns>
        T Update(T entity);

        /// <summary>
        /// Upserts the entities
        /// </summary>
        /// <param name="entities">The entities to update</param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// Deletes an entity from the repository by its id
        /// </summary>
        /// <param name="id">The string representation of the entity's id</param>
        void Delete(string id);

        /// <summary>
        /// Deletes an entity from the repository by its id
        /// </summary>
        /// <param name="id">The entity's id</param>
        void Delete(ObjectId id);

        /// <summary>
        /// Deletes the given entity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        void Delete(T entity);

        /// <summary>
        /// Deletes the entities matching the criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        void Delete(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Deletes all entities in the repository
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Counts the total entities in the repository
        /// </summary>
        /// <returns>Count of entities in the repository</returns>
        long Count();

        /// <summary>
        /// Checks if the entity exists for given criteria
        /// </summary>
        /// <typeparam name="T">The T</typeparam>
        /// <param name="criteria">The expression</param>
        /// <returns>true when an entity matching the criteria exists, false otherwise</returns>
        bool Exists(Expression<Func<T, bool>> criteria);
    }
}
