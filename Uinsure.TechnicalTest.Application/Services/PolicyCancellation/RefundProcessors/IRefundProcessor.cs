using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Entities;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;

public interface IRefundProcessor
{
    Payment Process(Policy policy, DateTimeOffset cancellationDate);
}
