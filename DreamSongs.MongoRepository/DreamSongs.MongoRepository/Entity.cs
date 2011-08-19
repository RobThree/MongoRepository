using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DreamSongs.MongoRepository
{
    /// <summary>
    /// Abstract Entity for all the BusinessEntities
    /// </summary>
    public abstract class Entity
    {
        protected Entity()
        {
            if (string.IsNullOrEmpty(CollectionName))
            {
                throw new ArgumentNullException("The collection name for this entity can't be empty.");
            }
        }

        protected Entity(string collectionName)
        {
            CollectionName = collectionName;            
        }

        /// <summary>
        /// Gets or sets the id for this object (the primary record for an entity)
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets the persistance collection name for this entity
        /// </summary>
        [BsonIgnore]
        public string CollectionName { get; private set; }
    }
}
