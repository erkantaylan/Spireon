namespace Core.Database.Models;

public interface IEntityFactory<out TEntity, in TCreate, in TUpdate>
    where TEntity : TCreate, TUpdate
    where TUpdate : TCreate
{
    static abstract TEntity Create(TCreate create);

    void Update(TUpdate update);
}