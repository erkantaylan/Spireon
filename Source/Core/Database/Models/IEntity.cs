namespace Core.Database.Models;

public interface IEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public interface IEntity<TKey> : IEntity where TKey : IComparable, IEquatable<TKey>
{
    TKey Id { get; set; }
}
