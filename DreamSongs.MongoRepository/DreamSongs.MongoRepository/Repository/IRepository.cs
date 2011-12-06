using System;
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
        /// Returns the T by its given ObjectId
        /// </summary>
        /// <param name="id">The string representing the ObjectId of the object to retrieve</param>
        /// <returns>The Entity T</returns>
        T GetById(string id);

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
        IQueryable<T> GetAll();

        /// <summary>
        /// Returns the list of T where it matches the criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        /// <returns>IQueryable of T</returns>
        IQueryable<T> GetAll(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Adds the new item in the collection
        /// </summary>
        /// <param name="item">The Item T</param>
        /// <returns>The added Item including its new ObjectId</returns>
        T Add(T item);

        /// <summary>
        /// Upserts an item
        /// </summary>
        /// <param name="item">The object</param>
        /// <returns>The updated object</returns>
        T Update(T item);

        /// <summary>
        /// Deletes an item from the collection by its id
        /// </summary>
        /// <param name="id">The string representation of the object id</param>
        void Delete(string id);

        /// <summary>
        /// Deletes an item from the collection by its id
        /// </summary>
        /// <param name="id">The object id</param>
        void Delete(ObjectId id);

        /// <summary>
        /// Counts the total items in the collection.
        /// </summary>
        /// <returns>Count of items in the collection</returns>
        long Count();

        /// <summary>
        /// Checks if the entity exists for given criteria
        /// </summary>
        /// <typeparam name="T">The T</typeparam>
        /// <param name="criteria">The expression</param>
        /// <returns>true when an entity matching the criteria exists, false otherwise</returns>
        bool Exists(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Returns an IQueryable for the given entity
        /// </summary>
        /// <returns>The IQueryable </returns>
        IQueryable<T> AsQueryable();
    }
}
