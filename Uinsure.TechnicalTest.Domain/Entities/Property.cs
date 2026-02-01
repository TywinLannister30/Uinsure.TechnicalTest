using Uninsure.TechnicalTest.Common;

namespace Uinsure.TechnicalTest.Domain.Entities;

public class Property : Entity<long>
{
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? AddressLine3 { get; private set; }
    public string? Postcode { get; private set; }
    public Guid PolicyId { get; private set; }

    public Property() { }

    public Property(string addressLine1, string addressLine2, string addressLine3, string postcode, Guid policyId)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        Postcode = postcode;
        PolicyId = policyId;
        CreatedDate = DateTimeOffset.UtcNow;
    }
}
