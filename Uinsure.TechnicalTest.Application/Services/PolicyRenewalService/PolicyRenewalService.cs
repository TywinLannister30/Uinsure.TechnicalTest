using Microsoft.Extensions.Options;
using Uinsure.TechnicalTest.Application.Configuration;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Application.Mappers;
using Uinsure.TechnicalTest.Domain.Enums;
using Uinsure.TechnicalTest.Domain.Repository;

namespace Uinsure.TechnicalTest.Application.Services.PolicyRenewalService;

public class PolicyRenewalService(IPolicyRepository policyRepository, IOptions<PolicySettings> policySettings) : IPolicyRenewalService
{
    private readonly IPolicyRepository _policyRepository = policyRepository;
    private readonly IOptions<PolicySettings> _policySettings = policySettings;

    public async Task<RenewPolicyResponseDto?> RenewPolicyAsync(Guid policyId, RenewPolicyRequestDto request)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId);

        if (policy is null)
            return null;

        if (policy.IsCancelled())
            return new RenewPolicyResponseDto { AlreadyCancelled = true };

        if (!policy.WithinRenewalWindow(_policySettings.Value.RenewalWindowDays, DateTimeOffset.UtcNow))
            return new RenewPolicyResponseDto { IsInRenewalWindow = false };

        if (policy.IsEnded(DateTimeOffset.UtcNow))
            return new RenewPolicyResponseDto { PolicyEnded = true };

        // This is Renew 3 a, and b. Only add a payment if policy is set to autorenew - not sure on the requirement here.
        // I have added a payment to the request assuming we would allow different payment methods between initial purchase and renewal.
        if (policy.AutoRenew)
            policy.AddPayment(request.Payment.ToDomain(policy.Id, TransactionType.Payment));

        policy.Renew();

        await _policyRepository.SaveChangesAsync();

        return new RenewPolicyResponseDto
        {
            Policy = policy.ToDto()        
        };
    }
}
