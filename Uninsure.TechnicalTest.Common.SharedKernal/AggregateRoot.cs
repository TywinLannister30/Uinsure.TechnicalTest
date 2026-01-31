using System.ComponentModel.DataAnnotations.Schema;

namespace Uninsure.TechnicalTest.Common.SharedKernal;

public abstract class AggregateRoot<T>: Entity<T>
{
    private readonly List<DomainEvent> _domainEvents = [];

    [NotMapped]
    public IEnumerable<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() { }

    protected AggregateRoot(T id) : base(id) { }

    protected void RegisterDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected void RemoveDomainEvent(DomainEvent domainEvent, string topic)
    {
        domainEvent.Topic = topic;
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
