using Uinsure.TechnicalTest.Domain.Agregates;
using Uinsure.TechnicalTest.Domain.Entities;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.RefundProcessors;

public interface IRefundProcessor
{
    Payment Process(Policy policy, DateTimeOffset cancellationDate);
}
