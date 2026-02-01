using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Uinsure.TechnicalTest.Application.Configuration;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.Application.Dtos.Api.Request;

public class CreatePolicyRequestDto : IValidatableObject
{
    public required string InsuranceType { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public bool AutoRenew { get; set; }
    public required List<PolicyholderDto> Policyholders { get; set; } = [];
    public required PropertyDto Property { get; set; }
    public required PaymentDto Payment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var settings = validationContext.GetService(typeof(IOptions<PolicySettings>)) as IOptions<PolicySettings>;

        var maxStartDateAdvanceDays = settings?.Value?.MaxStartDateAdvanceDays;
        var maxPolicyholders = settings?.Value?.MaxPolicyholders;

        if (string.IsNullOrWhiteSpace(InsuranceType))
            yield return new ValidationResult("InsuranceType is required.",[nameof(InsuranceType)]);
        else if (!Enum.TryParse<InsuranceType>(InsuranceType, true, out _))
            yield return new ValidationResult( $"InsuranceType must be one of: {string.Join(", ", Enum.GetNames<InsuranceType>())}",[nameof(InsuranceType)]);

        if (maxStartDateAdvanceDays == null || maxPolicyholders == null)
            throw new ArgumentNullException("PolicySettings configuration is missing.");

        if (StartDate.Date < DateTimeOffset.UtcNow.Date)
            yield return new ValidationResult("StartDate cannot be in the past.", [nameof(StartDate)]);

        if (StartDate.Date > DateTimeOffset.UtcNow.Date.AddDays(maxStartDateAdvanceDays.Value))
            yield return new ValidationResult($"StartDate cannot be more than {maxStartDateAdvanceDays} days in the future.", [nameof(StartDate)]);

        if (Policyholders == null)
            yield return new ValidationResult($"Policyholders cannot be null", [nameof(Policyholders)]);

        var policyholderCount = Policyholders?.Count ?? 0;
  
        if (policyholderCount < 1 || policyholderCount > maxPolicyholders)
            yield return new ValidationResult($"There must be between 1 and {maxPolicyholders} Policyholders",  [nameof(Policyholders)]);

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
