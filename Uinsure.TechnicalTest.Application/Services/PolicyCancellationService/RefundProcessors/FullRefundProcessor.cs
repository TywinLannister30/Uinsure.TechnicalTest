using Uinsure.TechnicalTest.Domain.Agregates;
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.RefundProcessors;

public class FullRefundProcessor : IRefundProcessor
{
    public Payment Process(Policy policy, DateTimeOffset cancellationDate)
    {
        var initialPayment = policy.Payments.OrderBy(x => x.CreatedDate).First(x => x.TransactionType == TransactionType.Payment);

        return new Payment(
            $"{initialPayment.PaymentReference}-Refund",
            initialPayment.Type,
            -Math.Abs(initialPayment.Amount),
            TransactionType.Refund,
            initialPayment.PolicyId);
    }
}
