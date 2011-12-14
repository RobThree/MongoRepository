using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DreamSongs.MongoRepository
{
    /// <summary>
    /// Entity interface
    /// </summary>
    public interface IEntity
    {
        [BsonId]
        string Id { get; set; }
    }

    /// <summary>
    /// Abstract Entity for all the BusinessEntities
    /// </summary>
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Gets or sets the id for this object (the primary record for an entity)
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }

    /// <summary>
    /// Attribute used to annotate Enities with to override mongo collection name. By default, when this attribute
    /// is not specified, the classname will be used
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CollectionName : Attribute
    {
        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a CollectionName attribute with the desired name
        /// </summary>
        /// <param name="value">Name of the collection</param>
        public CollectionName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty collectionname not allowed", "value");
            this.Name = value;
        }
    }
}
