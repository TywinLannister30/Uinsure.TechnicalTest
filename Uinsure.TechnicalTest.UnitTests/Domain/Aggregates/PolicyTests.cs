using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.UnitTests.Domain.Aggregates;

public class PolicyTests
{
    private readonly Policy _policy;

    public PolicyTests()
    {
        _policy = new Policy(DateTimeOffset.UtcNow, InsuranceType.Household, autoRenew: true);
    }

    [Fact]
    public void Ctr_NoAnomolies_Expect_ParametersToBeCorrect()
    {
        Assert.Equal(_policy.StartDate.AddYears(1), _policy.EndDate);
        Assert.Equal(PolicyState.Active, _policy.State);
        Assert.False(_policy.HasClaims);
    }

    [Fact]
    public void AddPolicyholder_NoAnomolies_Expect_PolicyholderAdded()
    {
        _policy.AddPolicyHolder(new Policyholder("FirstName", "LastName", DateTime.UtcNow, _policy.Id));
        
        Assert.Single(_policy.Policyholders);
    }

    [Fact]
    public void AddPayment_NoAnomolies_Expect_PaymentAddedAndAmountAltered()
    {
        var amount = 100;

        _policy.AddPayment(new Payment("PaymentReference", PaymentType.Cheque, amount, TransactionType.Payment, _policy.Id));

        Assert.Single(_policy.Payments);
        Assert.Equal(amount, _policy.Amount);
    }

    [Fact]
    public void AddProperty_NoAnomolies_Expect_PropertyAdded()
    {
        _policy.AddProperty(new Property("AddressLine1", "AddressLine2", "AddressLine3", "Postcode", _policy.Id));

        Assert.NotNull(_policy.Property);
    }

    [Fact]
    public void Cancel_NoAnomolies_Expect_PolicyCancelledAndCancellationDateAdded()
    {
        var cancellationDate = DateTimeOffset.UtcNow.AddMonths(1);

        _policy.Cancel(cancellationDate);

        Assert.Equal(PolicyState.Cancelled, _policy.State);
        Assert.Equal(cancellationDate, _policy.CancellationDate);
    }

    [Fact]
    public void IsCancelled_PolicyNotCancelled_Expect_False()
    {
        Assert.False(_policy.IsCancelled());
    }

    [Fact]
    public void IsCancelled_PolicyIsCancelled_Expect_True()
    {
        _policy.Cancel(DateTimeOffset.UtcNow);

        Assert.True(_policy.IsCancelled());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(30)]
    public void WithinRenewalWindow_PolicyIsWithinRenewalWindow_Expect_True(int numDays)
    {
        Assert.True(_policy.WithinRenewalWindow(30, _policy.EndDate.AddDays(-numDays)));
    }

    [Theory]
    [InlineData(31)]
    [InlineData(40)]
    [InlineData(100)]
    [InlineData(250)]
    public void WithinRenewalWindow_PolicyIsOutsideRenewalWindow_Expect_False(int numDays)
    {
        Assert.False(_policy.WithinRenewalWindow(30, _policy.EndDate.AddDays(-numDays)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(30)]
    public void IsEnded_PolicyHasEnded_Expect_True(int numDays)
    {
        Assert.True(_policy.IsEnded(_policy.EndDate.AddDays(numDays)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(30)]
    public void IsEnded_PolicyHasNotEnded_Expect_False(int numDays)
    {
        Assert.False(_policy.IsEnded(_policy.EndDate.AddDays(-numDays)));
    }

    [Fact]
    public void Renew_Expect_OneYearAddedToEndDate()
    {
        var previousEndDate = _policy.EndDate;

        _policy.Renew();

        Assert.Equal(previousEndDate.AddYears(1), _policy.EndDate);
    }

    [Fact]
    public void MarkAsClaim_Expect_HasClaims_True()
    {
        Assert.False(_policy.HasClaims);

        _policy.MarkAsClaim();

        Assert.True(_policy.HasClaims);
    }
}
