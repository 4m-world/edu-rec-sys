using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Domain.Common.Events;

namespace CodeMatrix.Mepd.Application.Common.Events;

public interface IEventService : ITransientService
{
    Task PublishAsync(DomainEvent domainEvent);
}