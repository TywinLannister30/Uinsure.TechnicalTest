using Uinsure.TechnicalTest.Domain.Agregates;
using Uinsure.TechnicalTest.Domain.Repository;
using Uinsure.TechnicalTest.Infrastructure.EntityFramework.Context;

namespace Uinsure.TechnicalTest.Infrastructure.EntityFramework.Repository;

public class PolicyRepository(IPolicyDbContext dbContext) : IPolicyRepository
{
    public async Task SaveAsync(Policy policy, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(policy);

        await dbContext.Policies.AddAsync(policy, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
