using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;
using Uninsure.TechnicalTest.Common;

namespace Uinsure.TechnicalTest.Domain.Agregates;

public class Policy : AggregateRoot<Guid>
{
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public PolicyState State { get; private set; }
    public InsuranceType InsuranceType { get; private set; }  
    public decimal Amount { get; private set; }
    public bool HasClaims { get; private set; }
    public bool AutoRenew { get; private set; }
    public List<Policyholder> Policyholders { get; private set; } = [];
    public Property? Property { get; private set; }
    public List<Payment> Payments { get; private set; } = [];
    public DateTimeOffset? CancellationDate { get; private set; }

    public Policy() { }

    public Policy(
        DateTimeOffset startDate, 
        InsuranceType insuranceType, 
        bool autoRenew): base()
    {
        Id = Guid.NewGuid();
        StartDate = startDate;
        EndDate = startDate.AddYears(1);
        State = PolicyState.Active;
        InsuranceType = insuranceType;
        HasClaims = false;
        AutoRenew = autoRenew;
        CreatedDate = DateTimeOffset.UtcNow;
    }

    public void AddPolicyHolder(Policyholder policyholder)
    {
        Policyholders.Add(policyholder);
    }

    public void AddPayment(Payment payment)
    {
        Payments.Add(payment);
        Amount += payment.Amount;
    }

    public void AddProperty(Property? property)
    {
        Property = property;
    }

    public void Cancel(DateTimeOffset cancellationDate)
    {
        State = PolicyState.Cancelled;
        CancellationDate = cancellationDate;
    }

    public bool IsCancelled()
    {
        return State == PolicyState.Cancelled;
    }

    public bool WithinRenewalWindow(int renewalWindowDays, DateTimeOffset now)
    {
        return EndDate >= now && EndDate <= now.AddDays(renewalWindowDays);
    }

    public bool IsEnded(DateTimeOffset now)
    {
        return EndDate < now;
    }

    public void Renew()
    {
        EndDate = EndDate.AddYears(1);
    }

    public void MarkAsClaim()
    {
        HasClaims = true;
    }

    public bool HasAnyClaims()
    {
        return HasClaims;
    }
}
