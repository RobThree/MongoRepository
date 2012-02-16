namespace DreamSongs.MongoRepository
{
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Entity interface.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the Id of the Entity.
        /// </summary>
        /// <value>Id of the Entity.</value>
        [BsonId]
        string Id { get; set; }
    }
}