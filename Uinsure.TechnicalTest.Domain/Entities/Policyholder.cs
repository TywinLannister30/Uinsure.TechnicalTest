using Uninsure.TechnicalTest.Common.SharedKernal;

namespace Uinsure.TechnicalTest.Domain.Entities;

public class Policyholder : Entity<long>
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTimeOffset DateOfBirth { get; private set; }
    public Guid PolicyId { get; private set; }

    public Policyholder() { }

    public Policyholder(string firstName, string lastName, DateTimeOffset dateOfBirth, Guid policyId)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        PolicyId = policyId;
    }
}
