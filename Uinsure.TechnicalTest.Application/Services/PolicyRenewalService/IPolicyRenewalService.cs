
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;

namespace Uinsure.TechnicalTest.Application.Services.PolicyRenewalService;

public interface IPolicyRenewalService
{
    Task<RenewPolicyResponseDto?> RenewPolicyAsync(Guid policyId, RenewPolicyRequestDto request);
}
