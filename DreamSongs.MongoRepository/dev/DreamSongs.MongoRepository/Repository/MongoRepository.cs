namespace DreamSongs.MongoRepository
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    public class MongoRepository<T> : IRepository<T>
        where T : IEntity
    {
        /// <summary>
        /// MongoCollection field.
        /// </summary>
        private MongoCollection<T> collection;

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public MongoRepository()
            : this(Util.GetDefaultConnectionString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public MongoRepository(string connectionString)
        {
            this.collection = Util.GetCollectionFromConnectionString<T>(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        public MongoRepository(MongoUrl url)
        {
            this.collection = Util.GetCollectionFromUrl<T>(url);
        }

        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations).
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class.
        /// </remarks>
        /// <value>The Mongo collection (to perform advanced operations).</value>
        [Obsolete("This property will be removed in future releases; for most purposes you can use the MongoRepositoryManager<T>.")]
        public MongoCollection<T> Collection
        {
            get
            {
                return this.collection;
            }
        }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The string representing the ObjectId of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public T GetById(string id)
        {
            if (typeof(T).IsSubclassOf(typeof(Entity)))
            {
                return this.collection.FindOneByIdAs<T>(new ObjectId(id));
            }

            return this.collection.FindOneByIdAs<T>(id);
        }

        /// <summary>
        /// Returns a single T by the given criteria.
        /// </summary>
        /// <param name="criteria">The expression.</param>
        /// <returns>A single T matching the criteria.</returns>
        public T GetSingle(Expression<Func<T, bool>> criteria)
        {
            return this.collection.AsQueryable<T>().Where(criteria).FirstOrDefault();
        }

        /// <summary>
        /// Returns the list of T where it matches the criteria.
        /// </summary>
        /// <param name="criteria">The expression.</param>
        /// <returns>IQueryable of T.</returns>
        public IQueryable<T> All(Expression<Func<T, bool>> criteria)
        {
            return this.collection.AsQueryable<T>().Where(criteria);
        }

        /// <summary>
        /// Returns All the records of T.
        /// </summary>
        /// <returns>IQueryable of T.</returns>
        public IQueryable<T> All()
        {
            return this.collection.AsQueryable<T>();
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public T Add(T entity)
        {
            this.collection.Insert<T>(entity);

            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public void Add(IEnumerable<T> entities)
        {
            this.collection.InsertBatch<T>(entities);
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public T Update(T entity)
        {
            this.collection.Save<T>(entity);

            return entity;
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                this.collection.Save<T>(entity);
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The string representation of the entity's id.</param>
        public void Delete(string id)
        {
            if (typeof(T).IsSubclassOf(typeof(Entity)))
            {
                this.collection.Remove(Query.EQ("_id", new ObjectId(id)));
            }
            else
            {
                this.collection.Remove(Query.EQ("_id", id));
            }
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(T entity)
        {
            this.Delete(entity.Id);
        }

        /// <summary>
        /// Deletes the entities matching the criteria.
        /// </summary>
        /// <param name="criteria">The expression.</param>
        public void Delete(Expression<Func<T, bool>> criteria)
        {
            foreach (T entity in this.collection.AsQueryable<T>().Where(criteria))
            {
                this.Delete(entity.Id);
            }
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public void DeleteAll()
        {
            this.collection.RemoveAll();
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public long Count()
        {
            return this.collection.Count();
        }

        /// <summary>
        /// Checks if the entity exists for given criteria.
        /// </summary>
        /// <param name="criteria">The expression.</param>
        /// <returns>true when an entity matching the criteria exists, false otherwise.</returns>
        public bool Exists(Expression<Func<T, bool>> criteria)
        {
            return this.collection.AsQueryable<T>().Any(criteria);
        }

        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        /// <remarks>
        ///     <para>
        ///         Sometimes a series of operations needs to be performed on the same connection in order to guarantee correct
        ///         results. This is rarely the case, and most of the time there is no need to call RequestStart/RequestDone.
        ///         An example of when this might be necessary is when a series of Inserts are called in rapid succession with
        ///         SafeMode off, and you want to query that data in a consistent manner immediately thereafter (with SafeMode
        ///         off the writes can queue up at the server and might not be immediately visible to other connections). Using
        ///         RequestStart you can force a query to be on the same connection as the writes, so the query won't execute
        ///         until the server has caught up with the writes.
        ///     </para>
        ///     <para>
        ///         A thread can temporarily reserve a connection from the connection pool by using RequestStart and
        ///         RequestDone. You are free to use any other databases as well during the request. RequestStart increments a
        ///         counter (for this thread) and RequestDone decrements the counter. The connection that was reserved is not
        ///         actually returned to the connection pool until the count reaches zero again. This means that calls to
        ///         RequestStart/RequestDone can be nested and the right thing will happen.
        ///     </para>
        /// </remarks>
        public IDisposable RequestStart()
        {
            return this.collection.Database.RequestStart();
        }
        
        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        /// <param name="slaveOk">Whether queries should be sent to secondary servers.</param>
        /// <remarks>
        ///     <para>
        ///         Sometimes a series of operations needs to be performed on the same connection in order to guarantee correct
        ///         results. This is rarely the case, and most of the time there is no need to call RequestStart/RequestDone.
        ///         An example of when this might be necessary is when a series of Inserts are called in rapid succession with
        ///         SafeMode off, and you want to query that data in a consistent manner immediately thereafter (with SafeMode
        ///         off the writes can queue up at the server and might not be immediately visible to other connections). Using
        ///         RequestStart you can force a query to be on the same connection as the writes, so the query won't execute
        ///         until the server has caught up with the writes.
        ///     </para>
        ///     <para>
        ///         A thread can temporarily reserve a connection from the connection pool by using RequestStart and
        ///         RequestDone. You are free to use any other databases as well during the request. RequestStart increments a
        ///         counter (for this thread) and RequestDone decrements the counter. The connection that was reserved is not
        ///         actually returned to the connection pool until the count reaches zero again. This means that calls to
        ///         RequestStart/RequestDone can be nested and the right thing will happen.
        ///     </para>
        /// </remarks>
        [Obsolete("Use the connectionstring to specify the readpreference; add \"readPreference=X\" where X is one of the following values: primary, primaryPreferred, secondary, secondaryPreferred, nearest. See http://docs.mongodb.org/manual/applications/replication/#read-preference")]
        public IDisposable RequestStart(bool slaveOk)
        {
            return this.collection.Database.RequestStart(slaveOk ? ReadPreference.SecondaryPreferred : ReadPreference.Primary);
        }

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
        public void RequestDone()
        {
            this.collection.Database.RequestDone();
        }
    }
}