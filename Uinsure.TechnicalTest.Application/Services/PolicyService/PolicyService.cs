
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Domain.Agregates;

namespace Uinsure.TechnicalTest.Application.Services.PolicyService;

public class PolicyService : IPolicyService
{
    public async Task<CreatePolicyResponseDto> CreatePolicyAsync(CreatePolicyRequestDto request)
    {
        var policy = new Policy
    }
}
