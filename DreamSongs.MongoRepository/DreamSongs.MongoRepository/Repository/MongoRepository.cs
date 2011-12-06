using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using FluentMongo.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DreamSongs.MongoRepository
{
    public class MongoRepository<T> : IRepository<T>
        where T : Entity
    {
        /// <summary>
        /// MongoCollection field
        /// </summary>
        private MongoCollection<T> _collection;

        /// <summary>
        /// Initilizes the instance of MongoRepository, Setups the MongoDB and the collection (i.e T)
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring</remarks>
        public MongoRepository()
            : this(ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString)
        { }

        /// <summary>
        /// Initilizes the instance of MongoRepository, Setups the MongoDB and The Collection (i.e T)
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB</param>
        public MongoRepository(string connectionString)
        {
            var collectionName = ((Entity)Activator.CreateInstance((typeof(T)))).CollectionName;
            if (String.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentException("Collection name cannot be empty for this entity");
            }

            var cnn = new MongoUrl(connectionString);
            var _server = MongoServer.Create(cnn.ToServerSettings());
            var _db = _server.GetDatabase(cnn.DatabaseName);
            _collection = _db.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations)
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class.
        /// </remarks>
        [Obsolete("This property will be removed in future releases.")]
        public MongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }

        /// <summary>
        /// Returns the T by its given ObjectId
        /// </summary>
        /// <param name="id">The string representing the ObjectId of the object to retrieve</param>
        /// <returns>The Entity T</returns>
        public T GetById(string id)
        {
            return _collection.FindOneByIdAs<T>(new ObjectId(id));
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
        public IQueryable<T> GetAll(Expression<Func<T, bool>> criteria)
        {
            return _collection.AsQueryable().Where(criteria);
        }

        /// <summary>
        /// Returns All the records of T
        /// </summary>
        /// <returns>IQueryable of T</returns>
        public IQueryable<T> GetAll()
        {
            return _collection.AsQueryable();
        }

        /// <summary>
        /// Adds the new item in the collection
        /// </summary>
        /// <param name="item">The Item T</param>
        /// <returns>The added Item including its new ObjectId</returns>
        public T Add(T item)
        {
            _collection.Insert<T>(item);

            return item;
        }

        /// <summary>
        /// Upserts an item
        /// </summary>
        /// <param name="item">The object</param>
        /// <returns>The updated object</returns>
        public T Update(T item)
        {
            _collection.Save<T>(item);

            return item;
        }

        /// <summary>
        /// Deletes an item from the collection by its id
        /// </summary>
        /// <param name="id">The string representation of the object id</param>
        public void Delete(string id)
        {
            this.Delete(new ObjectId(id));
        }

        /// <summary>
        /// Deletes an item from the collection by its id
        /// </summary>
        /// <param name="id">The object id</param>
        public void Delete(ObjectId id)
        {
            _collection.Remove(Query.EQ("_id", id));
        }

        /// <summary>
        /// Counts the total items in the collection.
        /// </summary>
        /// <returns>Count of items in the collection</returns>
        public long Count()
        {
            return _collection.Count();
        }

        /// <summary>
        /// Checks if the entity exists for given criteria
        /// </summary>
        /// <typeparam name="T">The T</typeparam>
        /// <param name="criteria">The expression</param>
        /// <returns>true when an entity matching the criteria exists, false otherwise</returns>
        public bool Exists(Expression<Func<T, bool>> criteria)
        {
            return _collection.AsQueryable().Any(criteria);
        }

        /// <summary>
        /// Returns an IQueryable for the given entity
        /// </summary>
        /// <returns>The IQueryable </returns>
        public IQueryable<T> AsQueryable()
        {
            return _collection.AsQueryable();
        }
    }
}

