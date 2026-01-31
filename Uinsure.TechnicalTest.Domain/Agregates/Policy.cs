using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;
using Uinsure.TechnicalTest.Domain.ValueObjects;
using Uninsure.TechnicalTest.Common.SharedKernal;

namespace Uinsure.TechnicalTest.Domain.Agregates;

public class Policy : AggregateRoot<string>
{
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public InsuranceType InsuranceType { get; private set; }  
    public decimal Amount { get; private set; }
    public bool HasClaims { get; private set; }
    public bool AutoRenew { get; private set; }
    public List<PolicyHolder> PolicyHolders { get; private set; }
    public Property? Property { get; private set; }
    public List<Payment> PaymentReferences { get; private set; } = [];

    public Policy() { }

    public Policy(
        DateTimeOffset startDate, 
        DateTimeOffset endDate, 
        InsuranceType insuranceType,
        decimal amount, 
        bool hasClaims, 
        bool autoRenew,
        List<PolicyHolder> policyHolders,
        Property property): base()
    {
        Id = Guid.NewGuid().ToString();
        StartDate = startDate;
        EndDate = endDate;
        InsuranceType = insuranceType;
        Amount = amount;
        HasClaims = hasClaims;
        AutoRenew = autoRenew;
        PolicyHolders = policyHolders;
        Property = property;
    }
}
