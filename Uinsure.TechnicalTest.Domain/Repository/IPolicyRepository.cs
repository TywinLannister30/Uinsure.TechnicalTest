using Uinsure.TechnicalTest.Domain.Agregates;

namespace Uinsure.TechnicalTest.Domain.Repository;

public interface IPolicyRepository
{
    Task SaveAsync(Policy policy, CancellationToken cancellationToken = default);
    Task<Policy?> GetByIdAsync(Guid policyId);
}
