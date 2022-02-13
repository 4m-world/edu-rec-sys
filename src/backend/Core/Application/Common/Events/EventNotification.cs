using CodeMatrix.Mepd.Domain.Common.Events;
using MediatR;

namespace CodeMatrix.Mepd.Application.Common.Events;

public class EventNotification<T> : INotification
where T : DomainEvent
{
    public EventNotification(T domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public T DomainEvent { get; }
}