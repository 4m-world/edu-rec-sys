using CodeMatrix.Mepd.Domain.Common.Contracts;

namespace CodeMatrix.Mepd.Domain.Common.Events;

public class EntityDeletedEvent<T> : DomainEvent
    where T : IEntity
{
    public EntityDeletedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; set; }
}

public static class EntityDeletedEvent
{
    public static EntityDeletedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
        where TEntity : IEntity
    {
        return new(entity);
    }
}