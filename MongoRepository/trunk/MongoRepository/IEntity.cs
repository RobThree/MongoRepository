namespace MongoRepository
{
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Generic Entity interface.
    /// </summary>
    public interface IEntity<T>
    {
        /// <summary>
        /// Gets or sets the Id of the Entity.
        /// </summary>
        /// <value>Id of the Entity.</value>
        [BsonId]
        T Id { get; set; }
    }

    /// <summary>
    /// "Default" Entity interface.
    /// </summary>
    public interface IEntity : IEntity<string>
    {
    }
}