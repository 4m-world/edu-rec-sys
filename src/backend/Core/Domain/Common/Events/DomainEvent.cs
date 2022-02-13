namespace CodeMatrix.Mepd.Domain.Common.Events;

public abstract class DomainEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}