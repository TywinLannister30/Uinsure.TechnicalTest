using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.UnitTests.Helpers;

public static class PolicyHelpers
{
    public static Policy CreatePolicy(bool isCancelled = false, bool hasClaims = false, decimal amount = 100, DateTimeOffset? startDate = null, bool autoRenew = true)
    {
        var policy = new Policy(
            startDate ?? new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero),
            InsuranceType.Household,
            autoRenew
        );

        policy.AddPayment(new Payment(
            "reference",
            PaymentType.Card,
            amount,
            TransactionType.Payment,
            policy.Id
            ));

        if (hasClaims)
            policy.MarkAsClaim();

        if (isCancelled)
            policy.Cancel(new DateTimeOffset(2029, 6, 1, 0, 0, 0, TimeSpan.Zero));

        return policy;
    }

    public static Payment CreatePayment(decimal amount, TransactionType transactionType, Guid policyId)
    {
        return new Payment(
            "reference",
            PaymentType.Card,
            amount,
            transactionType,
            policyId
        );
    }
}
