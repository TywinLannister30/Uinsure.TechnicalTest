using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyCancellation.RefundProcessors;

public class FullRefundProcessorTests
{
    private static FullRefundProcessor CreateSut() => new();

    [Fact]
    public void Process_UsesEarliestPayment_And_ReturnsFullNegativeRefund()
    {
        var amount = 100;

        var policy = new Policy(
            new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero),
            InsuranceType.Household,
            autoRenew: true
            );

        policy.AddPayment(
            new Payment(
                "paymentReference",
                PaymentType.Card,
                amount,
                TransactionType.Payment,
                policy.Id));

        var refund = CreateSut().Process(policy, cancellationDate: new DateTimeOffset(2026, 01, 10, 0, 0, 0, TimeSpan.Zero));

        Assert.Equal("paymentReference-Refund", refund.PaymentReference);
        Assert.Equal(TransactionType.Refund, refund.TransactionType);
        Assert.Equal(PaymentType.Card, refund.Type);
        Assert.Equal(-amount, refund.Amount);
        Assert.Equal(policy.Id, refund.PolicyId);
    }
}
