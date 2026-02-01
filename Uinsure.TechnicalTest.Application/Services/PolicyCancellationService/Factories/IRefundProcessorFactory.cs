using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.RefundProcessors;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.Factories;

public interface IRefundProcessorFactory
{
    IRefundProcessor GetRefundProcessor(DateTimeOffset policyStartDate, DateTimeOffset policyCancellationDate);
}
