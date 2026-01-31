namespace Uninsure.TechnicalTest.Common.SharedKernal;

public abstract class DomainEvent
{
    public Guid EventId { get; private set; } = Guid.NewGuid();

    public DateTime EventDateTime { get; protected set; } = DateTime.UtcNow;

    public string Topic { get; internal set; }
}
