using Uinsure.TechnicalTest.Application.Dtos;

namespace Uinsure.TechnicalTest.Application.Services.PolicyRetrievalService;

public interface IPolicyRetrievalService
{
    Task<PolicyDto?> GetPolicyAsync(Guid policyId);
}
