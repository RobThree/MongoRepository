using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DreamSongs.MongoRepository
{
    /// <summary>
    /// Abstract Entity for all the BusinessEntities
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Gets or sets the id for this object (the primary record for an entity)
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets the persistance collection name for this entity
        /// </summary>
        [BsonIgnore]
        public string CollectionName { get; private set; }

        /// <summary>
        /// Default constructor; uses the typename as collectionname
        /// </summary>
        protected Entity()
        {
            CollectionName = this.GetType().Name;
        }

        /// <summary>
        /// Constructor specifying collectionname
        /// </summary>
        /// <param name="collectionName">Used to override collectionname</param>
        protected Entity(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
