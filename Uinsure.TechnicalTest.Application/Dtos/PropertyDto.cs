using Uinsure.TechnicalTest.Domain.Entities;

namespace Uinsure.TechnicalTest.Application.Dtos;

public class PropertyDto
{
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string AddressLine3 { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;

    public Property ToDomain(Guid policyId)
    {
        return new Property(AddressLine1, AddressLine2, AddressLine3, Postcode, policyId);
    }
}
