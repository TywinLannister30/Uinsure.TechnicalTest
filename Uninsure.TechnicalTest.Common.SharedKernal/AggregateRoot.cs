namespace Uninsure.TechnicalTest.Common.SharedKernal;

public abstract class AggregateRoot<T>: Entity<T>
{

    protected AggregateRoot() { }

    protected AggregateRoot(T id) : base(id) { }
}
