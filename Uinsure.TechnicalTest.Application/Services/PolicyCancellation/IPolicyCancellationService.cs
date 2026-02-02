using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellation;

public interface IPolicyCancellationService
{
    Task<CancelPolicyResponseDto?> CancelPolicyAsync(Guid policyId, CancelPolicyRequestDto request, bool actionRefund = true);
}
