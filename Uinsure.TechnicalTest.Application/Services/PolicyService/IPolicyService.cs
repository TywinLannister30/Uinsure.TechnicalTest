using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;

namespace Uinsure.TechnicalTest.Application.Services.PolicyService;

public interface IPolicyService
{
    Task<CreatePolicyResponseDto> CreatePolicyAsync(CreatePolicyRequestDto request);
}
