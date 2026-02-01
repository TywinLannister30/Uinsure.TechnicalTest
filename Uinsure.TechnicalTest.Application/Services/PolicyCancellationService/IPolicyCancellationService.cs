using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellationService;

public interface IPolicyCancellationService
{
    Task<CancelPolicyResponseDto?> CancelPolicyAsync(Guid policyId, CancelPolicyRequestDto request);
}
