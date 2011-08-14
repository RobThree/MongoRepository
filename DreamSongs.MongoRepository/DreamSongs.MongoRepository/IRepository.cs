using System;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace DreamSongs.MongoRepository
{
   public interface IRepository<T> where T : Entity
    {

       /// <summary>
       /// Gets the Mongo collection (to perform advance operations)
       /// </summary>
        MongoCollection<T> Collection { get; }

       /// <summary>
       /// Gets the database in being used for this repository
       /// </summary>
        MongoDatabase DB { get; }

        /// <summary>
        /// Returns the T by its given ObjectId
        /// </summary>
        /// <param name="id">The object Id</param>
        /// <returns>The Entity T</returns>
        T GetById(string id);

        /// <summary>
        /// Returns the T (1 record) by the given criteria
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>The T</returns>
        T GetSingle(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Retunrs the list of T where it matches the criteria
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>List of T</returns>
        IQueryable<T> GetAll(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Inserts the new item in DB
        /// </summary>
        /// <param name="item">The Item T</param>
        /// <returns>The added Item inclduing its new ObjectId</returns>
        T Insert(T item);

        /// <summary>
        /// Updates a row
        /// </summary>
        /// <param name="item">The object</param>
        /// <returns>The updated object</returns>
        T Update(T item);

       /// <summary>
       /// Deletes a document from db by its id
       /// </summary>
       /// <param name="objectId">The obj id</param>
        void Remove(string objectId);
    }
}
