
namespace Uinsure.TechnicalTest.Domain.ValueObjects;

public class PolicyHolder : ValueObject
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTimeOffset DateOfBirth { get; private set; }

    public PolicyHolder() { }

    public PolicyHolder(string firstName, string lastName, DateTimeOffset dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }
}
