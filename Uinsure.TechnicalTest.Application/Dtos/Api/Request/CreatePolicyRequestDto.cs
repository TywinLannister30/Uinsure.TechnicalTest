using System.ComponentModel.DataAnnotations;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.Application.Dtos.Api.Request;

public class CreatePolicyRequestDto : IValidatableObject
{
    public required InsuranceType InsuranceType { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public bool AutoRenew { get; set; }
    public required List<PolicyholderDto> Policyholders { get; set; } = [];
    public required PropertyDto Property { get; set; }
    public required PaymentDto Payment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.Date < DateTimeOffset.UtcNow.Date)
            yield return new ValidationResult("StartDate cannot be in the past.", [nameof(StartDate)]);

        if (StartDate.Date > DateTimeOffset.UtcNow.Date.AddDays(60)) //todo: move to config
            yield return new ValidationResult("StartDate cannot be more than 60 days in the future.", [nameof(StartDate)]);

        var policyholderCount = Policyholders?.Count ?? 0;
        
        if (policyholderCount < 1 || policyholderCount > 3)
            yield return new ValidationResult("There must be between 1 and 3 Policyholders",  [nameof(Policyholders)]);

        if (policyholderCount > 0)
        {
            for (var i = 0; i < policyholderCount; i++)
            {
                var dateOfBirth = Policyholders[i].DateOfBirth.Date;
                var age = StartDate.Year - dateOfBirth.Year;
                
                if (dateOfBirth > StartDate.AddYears(-age))
                    age--;

                if (age < 16) 
                    yield return new ValidationResult("All policyholders must be at least 16.",[$"{nameof(Policyholders)}[{i}].{nameof(PolicyholderDto.DateOfBirth)}"]);
            }
        }
    }
}
