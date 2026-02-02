using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Mappers;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Enums;
using Uinsure.TechnicalTest.Domain.Repository;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCreation;

public class PolicyCreationService(IPolicyRepository policyRepository) : IPolicyCreationService
{
    private readonly IPolicyRepository _policyRepository = policyRepository;

    public async Task<PolicyDto> CreatePolicyAsync(CreatePolicyRequestDto request)
    {
        var policy = new Policy(
            request.StartDate,
            Enum.Parse<InsuranceType>(request.InsuranceType, ignoreCase: true),
            request.AutoRenew);

        foreach (var policyholder in request.Policyholders)
            policy.AddPolicyHolder(policyholder.ToDomain(policy.Id));

        policy.AddPayment(request.Payment.ToDomain(policy.Id, TransactionType.Payment));
        policy.AddProperty(request.Property?.ToDomain(policy.Id));

        await _policyRepository.SaveAsync(policy);

        return policy.ToDto();
    }
}
