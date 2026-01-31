using Uinsure.TechnicalTest.Domain.Entities;

namespace Uinsure.TechnicalTest.Application.Dtos;

public class PolicyholderDto
{
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required DateTime DateOfBirth { get; set; }

    internal Policyholder ToDomain(Guid policyId)
    {
        return new Policyholder(FirstName, LastName, new DateTimeOffset(DateOfBirth), policyId);
    }
}
