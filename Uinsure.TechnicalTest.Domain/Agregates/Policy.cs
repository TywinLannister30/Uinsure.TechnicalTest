using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.ValueObjects;
using Uninsure.TechnicalTest.Common.SharedKernal;

namespace Uinsure.TechnicalTest.Domain.Agregates;

public class Policy : AggregateRoot<string>
{
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public decimal Amount { get; private set; }
    public bool HasClaims { get; private set; }
    public bool AutoRenew { get; private set; }
    public PolicyHolder? PolicyHolder { get; private set; }
    public Property? Property { get; private set; }
    public List<PaymentReference> PaymentReferences { get; private set; } = [];

    public Policy() { }

    public Policy(
        DateTimeOffset startDate, 
        DateTimeOffset endDate, 
        decimal amount, 
        bool hasClaims, 
        bool autoRenew,
        PolicyHolder policyHolder,
        Property property): base()
    {
        Id = Guid.NewGuid().ToString();
        StartDate = startDate;
        EndDate = endDate;
        Amount = amount;
        HasClaims = hasClaims;
        AutoRenew = autoRenew;
        PolicyHolder = policyHolder;
        Property = property;
    }
}
