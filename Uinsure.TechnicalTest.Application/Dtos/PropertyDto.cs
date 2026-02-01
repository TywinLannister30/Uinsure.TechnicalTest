using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Uinsure.TechnicalTest.Application.Configuration;
using Uinsure.TechnicalTest.Domain.Entities;

namespace Uinsure.TechnicalTest.Application.Dtos;

public class PropertyDto : IValidatableObject
{
    [Required(AllowEmptyStrings = false)]
    public required string AddressLine1 { get; set; } = string.Empty;
    
    public string AddressLine2 { get; set; } = string.Empty;
    
    public string AddressLine3 { get; set; } = string.Empty;
    
    [Required(AllowEmptyStrings = false)]
    public required string Postcode { get; set; } = string.Empty;

    public Property ToDomain(Guid policyId)
    {
        return new Property(AddressLine1, AddressLine2, AddressLine3, Postcode, policyId);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var settings = validationContext.GetService(typeof(IOptions<PolicySettings>)) as IOptions<PolicySettings>;

        var maxPostcodeLength = settings?.Value?.MaxPostcodeLength;

        if (maxPostcodeLength == null)
            throw new ArgumentNullException("PolicySettings configuration is missing.");

        if (Postcode.Length > maxPostcodeLength)
            yield return new ValidationResult($"Postcode can be no longer than {maxPostcodeLength} characters.", [nameof(Postcode)]);

        // other postcode validation...
    }
}
