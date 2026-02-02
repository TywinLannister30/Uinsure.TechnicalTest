using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Mappers;
using Uinsure.TechnicalTest.Domain.Repository;

namespace Uinsure.TechnicalTest.Application.Services.PolicyRetrieval;

public class PolicyRetrievalService(IPolicyRepository policyRepository) : IPolicyRetrievalService
{
    private readonly IPolicyRepository _policyRepository = policyRepository;

    public async Task<PolicyDto?> GetPolicyAsync(Guid policyId)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId);

        if (policy is null)
            return null;

        return policy.ToDto();
    }
}
