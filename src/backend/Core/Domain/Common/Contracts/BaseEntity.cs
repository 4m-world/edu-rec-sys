using CodeMatrix.Mepd.Domain.Common.Events;
using MassTransit;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMatrix.Mepd.Domain.Common.Contracts;

public abstract class BaseEntity<T> : IEntity<T>
{
    public T Id { get; protected set; } = default;

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new();
}

public abstract class BaseEntity : BaseEntity<string>
{
    protected BaseEntity()
    {
        Id = NewId.Next().ToGuid().ToString();
    }
}