using Uinsure.TechnicalTest.Domain.Aggregates;

namespace Uinsure.TechnicalTest.Domain.Repository;

public interface IPolicyRepository
{
    Task SaveAsync(Policy policy, CancellationToken cancellationToken = default);
    Task<Policy?> GetByIdAsync(Guid policyId);
    Task SaveChangesAsync();
}
