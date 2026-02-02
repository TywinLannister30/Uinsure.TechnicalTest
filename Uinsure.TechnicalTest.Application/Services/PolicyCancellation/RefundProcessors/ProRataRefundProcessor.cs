using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;

public class ProRataRefundProcessor : IRefundProcessor
{
    public Payment Process(Policy policy, DateTimeOffset cancellationDate)
    {
        // Messy calculator here - just assuming we want to divide up the year and refund evenly for unused.
        var initialPayment = policy.Payments.OrderBy(x => x.CreatedDate).First(x => x.TransactionType == TransactionType.Payment);
        var premium = Math.Abs(initialPayment.Amount);

        var start = policy.StartDate.UtcDateTime.Date;
        var end = policy.EndDate.UtcDateTime.Date;
        var cancel = cancellationDate.UtcDateTime.Date;

        var totalDays = (end - start).Days;

        var unusedDays = cancel <= start 
            ? totalDays 
            : cancel >= end 
                ? 0 
                : (end - cancel).Days;

        var refund = premium * unusedDays / totalDays;

        refund = decimal.Round(refund, 2, MidpointRounding.AwayFromZero);

        return new Payment(
            $"{initialPayment.PaymentReference}-Refund",
            initialPayment.Type,
            -Math.Abs(refund),
            TransactionType.Refund,
            initialPayment.PolicyId);
    }
}
