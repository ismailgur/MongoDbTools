namespace MongoDbTools.MongoDb
{
    public interface IEntity
    {

    }


    public interface IEntity<out TKey> : IEntity where TKey : IEquatable<TKey>
    {
        public TKey Id { get; }
        public DateTime CreatedAt { get; set; }
    }
}