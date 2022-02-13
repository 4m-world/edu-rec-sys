using CodeMatrix.Mepd.Domain.Common.Events;

namespace CodeMatrix.Mepd.Domain.Common.Contracts;

public interface IEntity
{
    List<DomainEvent> DomainEvents { get; }
}

public interface IEntity<T> : IEntity
{
    T Id { get; }
}
