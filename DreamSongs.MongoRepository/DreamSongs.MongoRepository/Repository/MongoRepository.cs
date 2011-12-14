using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentMongo.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DreamSongs.MongoRepository
{
    /// <summary>
    /// Deals with entities in MongoDb
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoRepository<T> : IRepository<T>
        where T : IEntity
    {
        /// <summary>
        /// MongoCollection field
        /// </summary>
        private MongoCollection<T> _collection;

        /// <summary>
        /// Initilizes the instance of MongoRepository, Setups the MongoDB and the repository (i.e T)
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring</remarks>
        public MongoRepository()
            : this(Util.GetDefaultConnectionString())
        { }

        /// <summary>
        /// Initilizes the instance of MongoRepository, Setups the MongoDB and the repository (i.e T)
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB</param>
        public MongoRepository(string connectionString)
        {
            _collection = Util.GetCollectionFromConnectionString<T>(connectionString);
        }

        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations)
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class.
        /// </remarks>
        [Obsolete("This property will be removed in future releases; for most purposes you can use the MongoRepositoryManager<T>.")]
        public MongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }

        /// <summary>
        /// Returns the T by its given id
        /// </summary>
        /// <param name="id">The string representing the ObjectId of the entity to retrieve</param>
        /// <returns>The Entity T</returns>
        public T GetById(string id)
        {
            if (typeof(T).IsSubclassOf(typeof(Entity)))
                return this.GetById(new ObjectId(id));
            return _collection.FindOneByIdAs<T>(id);
        }

        /// <summary>
        /// Returns the T by its given ObjectId
        /// </summary>
        /// <param name="id">The string representing the ObjectId of the entity to retrieve</param>
        /// <returns>The Entity T</returns>
        public T GetById(ObjectId id)
        {
            return _collection.FindOneByIdAs<T>(id);
        }

        /// <summary>
        /// Returns a single T by the given criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        /// <returns>The T</returns>
        public T GetSingle(Expression<Func<T, bool>> criteria)
        {
            return _collection.AsQueryable().Where(criteria).FirstOrDefault();
        }

        /// <summary>
        /// Returns the list of T where it matches the criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        /// <returns>IQueryable of T</returns>
        public IQueryable<T> All(Expression<Func<T, bool>> criteria)
        {
            return _collection.AsQueryable().Where(criteria);
        }

        /// <summary>
        /// Returns All the records of T
        /// </summary>
        /// <returns>IQueryable of T</returns>
        public IQueryable<T> All()
        {
            return _collection.AsQueryable();
        }

        /// <summary>
        /// Adds the new entity in the repository
        /// </summary>
        /// <param name="entity">The entity T</param>
        /// <returns>The added entity including its new ObjectId</returns>
        public T Add(T entity)
        {
            _collection.Insert<T>(entity);

            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository
        /// </summary>
        /// <param name="entities">The entities of type T</param>
        public void Add(IEnumerable<T> entities)
        {
            _collection.InsertBatch<T>(entities);
        }

        /// <summary>
        /// Upserts an entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The updated entity</returns>
        public T Update(T entity)
        {
            _collection.Save<T>(entity);

            return entity;
        }

        /// <summary>
        /// Upserts the entities
        /// </summary>
        /// <param name="entities">The entities to update</param>
        public void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                _collection.Save<T>(entity);
        }

        /// <summary>
        /// Deletes an entity from the repository by its id
        /// </summary>
        /// <param name="id">The string representation of the entity's id</param>
        public void Delete(string id)
        {
            this.Delete(new ObjectId(id));
        }

        /// <summary>
        /// Deletes an entity from the repository by its id
        /// </summary>
        /// <param name="id">The entity's id</param>
        public void Delete(ObjectId id)
        {
            _collection.Remove(Query.EQ("_id", id));
        }

        /// <summary>
        /// Deletes the given entity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        public void Delete(T entity)
        {
            this.Delete(entity.Id);
        }

        /// <summary>
        /// Deletes the entities matching the criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        public void Delete(Expression<Func<T, bool>> criteria)
        {
            foreach (T entity in _collection.AsQueryable().Where(criteria))
                _collection.Remove(Query.EQ("_id", new ObjectId(entity.Id)));
        }

        /// <summary>
        /// Deletes all entities in the repository
        /// </summary>
        public void DeleteAll()
        {
            _collection.RemoveAll();
        }

        /// <summary>
        /// Counts the total entities in the repository
        /// </summary>
        /// <returns>Count of entities in the collection</returns>
        public long Count()
        {
            return _collection.Count();
        }

        /// <summary>
        /// Checks if the entity exists for given criteria
        /// </summary>
        /// <param name="criteria">The expression</param>
        /// <returns>true when an entity matching the criteria exists, false otherwise</returns>
        public bool Exists(Expression<Func<T, bool>> criteria)
        {
            return _collection.AsQueryable().Any(criteria);
        }
    }
}

