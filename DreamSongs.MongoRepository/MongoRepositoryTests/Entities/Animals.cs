using DreamSongs.MongoRepository;

namespace MongoRepositoryTests.Entities
{
    [CollectionName("AnimalsTest")]
    public abstract class Animal : Entity { }

    [CollectionName("Catlikes")]
    public abstract class CatLike : Animal { }

    [CollectionName("Birds")]
    public class Bird : Animal { }

    public class Lion : CatLike { }

    public class Cat : CatLike { }

    public class Dog : Animal { }

    public class Parrot : Bird { }

    public class Macaw : Parrot { }

    public class Whale : Entity { }
}
