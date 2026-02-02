using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyCancellation.RefundProcessors;

public class ProRataRefundProcessorTests
{
    private static ProRataRefundProcessor CreateSut() => new();

    [Fact]
    public void Process_WhenCancelledMidTerm_RefundsUnusedPortion_RoundedTo2dp_AwayFromZero()
    {
        var cancellationDate = new DateTimeOffset(2026, 06, 01, 0, 0, 0, TimeSpan.Zero);

        var policy = new Policy(
            new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero),
            InsuranceType.Household,
            autoRenew: true
            );

        policy.AddPayment(
            new Payment(
                "paymentReference",
                PaymentType.Card,
                99.99m,
                TransactionType.Payment,
                policy.Id));

        var refund = CreateSut().Process(policy, cancellationDate);

        Assert.Equal("paymentReference-Refund", refund.PaymentReference);
        Assert.Equal(TransactionType.Refund, refund.TransactionType);
        Assert.Equal(policy.Id, refund.PolicyId);
        Assert.Equal(PaymentType.Card, refund.Type);
        Assert.Equal(-58.62m, refund.Amount);
    }

    [Fact]
    public void Process_WhenCancelledOnOrBeforeStart_RefundsFullPremium()
    {
        var policy = new Policy(
            new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero),
            InsuranceType.Household,
            autoRenew: true
            );

        policy.AddPayment(
            new Payment(
                "paymentReference",
                PaymentType.Card,
                99.99m,
                TransactionType.Payment,
                policy.Id));

        var refund = CreateSut().Process(policy, cancellationDate: policy.StartDate);

        Assert.Equal(-99.99m, refund.Amount);
        Assert.Equal(TransactionType.Refund, refund.TransactionType);
    }

    [Fact]
    public void Process_WhenCancelledOnOrAfterEnd_RefundsZero()
    {
        var policy = new Policy(
            new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero),
            InsuranceType.Household,
            autoRenew: true
            );

        policy.AddPayment(
            new Payment(
                "paymentReference",
                PaymentType.Card,
                99.99m,
                TransactionType.Payment,
                policy.Id));

        var refund = CreateSut().Process(policy, cancellationDate: policy.EndDate);

        Assert.Equal(0.00m, refund.Amount);
        Assert.Equal(TransactionType.Refund, refund.TransactionType);
    }
}
