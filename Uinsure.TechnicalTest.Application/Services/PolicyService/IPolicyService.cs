using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;

namespace Uinsure.TechnicalTest.Application.Services.PolicyService;

public interface IPolicyService
{
    Task<PolicyDto> CreatePolicyAsync(CreatePolicyRequestDto request);
    Task<PolicyDto> GetPolicyAsync(Guid policyId);
}
