namespace DreamSongs.MongoRepository
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Abstract Entity for all the BusinessEntities.
    /// </summary>
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Gets or sets the id for this object (the primary record for an entity).
        /// </summary>
        /// <value>Thet id for this object (the primary record for an entity).</value>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
