namespace Core.Database.Models;

public abstract class EntityBase<TKey> : IEntity<TKey> where TKey : IComparable, IEquatable<TKey>
{
    public TKey Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}