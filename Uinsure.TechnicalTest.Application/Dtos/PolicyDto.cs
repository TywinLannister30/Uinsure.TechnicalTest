namespace Uinsure.TechnicalTest.Application.Dtos;

public class PolicyDto
{
    public Guid UniqueReference { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string InsuranceType { get; set; }
    public decimal Amount { get; set; }
    public bool HasClaims { get; set; }
    public bool AutoRenew { get; set; }
    public List<PolicyholderDto> Policyholders { get; set; } = [];
    public PropertyDto? Property { get; set; }
    public List<PaymentDto> Payments { get; set; } = [];
}
