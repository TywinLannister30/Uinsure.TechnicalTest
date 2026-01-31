using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Domain.Agregates;
using Uinsure.TechnicalTest.Domain.Repository;

namespace Uinsure.TechnicalTest.Application.Services.PolicyService;

public class PolicyService(IPolicyRepository policyRepository) : IPolicyService
{
    private readonly IPolicyRepository _policyRepository = policyRepository;

    public async Task<CreatePolicyResponseDto> CreatePolicyAsync(CreatePolicyRequestDto request)
    {
        var policy = new Policy(
            request.StartDate,
            request.InsuranceType,
            request.Payment.Amount,
            false,
            request.AutoRenew
            );

        foreach (var policyholder in request.Policyholders)
            policy.AddPolicyHolder(policyholder.ToDomain(policy.Id));

        policy.AddPayment(request.Payment.ToDomain(policy.Id));
        policy.AddProperty(request.Property?.ToDomain(policy.Id));

        await _policyRepository.SaveAsync(policy);

        return null;
    }
}
