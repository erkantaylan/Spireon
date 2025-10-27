namespace Core.Database.Models;

public interface IHaveId<TKey> where TKey : IComparable, IEquatable<TKey>
{
    TKey Id { get; set; }
}