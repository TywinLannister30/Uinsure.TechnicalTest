using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Mappers;
using Uinsure.TechnicalTest.Domain.Repository;

namespace Uinsure.TechnicalTest.Application.Services.PolicyClaim;

public class PolicyClaimService(IPolicyRepository policyRepository) : IPolicyClaimService
{
    private readonly IPolicyRepository _policyRepository = policyRepository;

    public async Task<PolicyDto?> MarkAsClaim(Guid policyId)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId);

        if (policy is null)
            return null;

        policy.MarkAsClaim();

        await _policyRepository.SaveChangesAsync();

        return policy.ToDto();
    }
}
