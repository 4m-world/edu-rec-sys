using CodeMatrix.Mepd.Domain.Common.Contracts;

namespace CodeMatrix.Mepd.Domain.Common.Events;

public class EntityCreatedEvent<T> : DomainEvent
    where T : IEntity
{
    public EntityCreatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; set; }
}

public static class EntityCreatedEvent
{
    public static EntityCreatedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
        where TEntity : IEntity
    {
        return new(entity);
    }
}