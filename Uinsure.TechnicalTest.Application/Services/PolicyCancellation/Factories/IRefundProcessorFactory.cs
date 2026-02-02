using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellation.Factories;

public interface IRefundProcessorFactory
{
    IRefundProcessor GetRefundProcessor(DateTimeOffset policyStartDate, DateTimeOffset policyCancellationDate);
}
