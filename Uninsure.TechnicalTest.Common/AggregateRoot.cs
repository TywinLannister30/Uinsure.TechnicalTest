namespace Uninsure.TechnicalTest.Common;

public abstract class AggregateRoot<T>: Entity<T>
{

    protected AggregateRoot() { }

    protected AggregateRoot(T id) : base(id) { }
}
