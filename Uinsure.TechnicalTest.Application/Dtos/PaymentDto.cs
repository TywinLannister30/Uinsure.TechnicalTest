using System.ComponentModel.DataAnnotations;
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.Application.Dtos;

public class PaymentDto : IValidatableObject
{
    public string Reference { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    internal Payment ToDomain(Guid policyId)
    {
        return new Payment(Reference, Enum.Parse<PaymentType>(PaymentType, ignoreCase: true), Amount, policyId);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(PaymentType))
            yield return new ValidationResult("PaymentType is required.", [nameof(PaymentType)]);
        else if (!Enum.TryParse<PaymentType>(PaymentType, true, out _))
            yield return new ValidationResult($"PaymentType must be one of: {string.Join(", ", Enum.GetNames<PaymentType>())}", [nameof(PaymentType)]);
    }
}
