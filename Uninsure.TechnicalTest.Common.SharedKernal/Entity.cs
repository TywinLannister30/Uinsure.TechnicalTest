namespace Uninsure.TechnicalTest.Common.SharedKernal;

public abstract class Entity<T>
{
    public T Id { get; protected set; }
    public DateTimeOffset CreatedDate { get; private set; }

    protected Entity() { }

    protected Entity(T id) 
    { 
        Id = id;
        CreatedDate = DateTimeOffset.UtcNow;
    }
}
