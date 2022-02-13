using CodeMatrix.Mepd.Domain.Common.Contracts;

namespace CodeMatrix.Mepd.Domain.Common.Events;

public class EntityUpdatedEvent<T> : DomainEvent
    where T : IEntity
{
    public EntityUpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; set; }
}


public static class EntityUpdatedEvent
{
    public static EntityUpdatedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
        where TEntity : IEntity
    {
        return new(entity);
    }
}