using Uinsure.TechnicalTest.Domain.ValueObjects;

namespace Uinsure.TechnicalTest.Application.Dtos;

public class PolicyholderDto
{
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required DateTime DateOfBirth { get; set; }

    internal PolicyHolder ToDomain()
    {
        return new PolicyHolder(FirstName, LastName, new DateTimeOffset(DateOfBirth));
    }
}
