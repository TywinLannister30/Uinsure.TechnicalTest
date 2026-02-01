using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCreationService;

public interface IPolicyCreationService
{
    Task<PolicyDto> CreatePolicyAsync(CreatePolicyRequestDto request);
}
