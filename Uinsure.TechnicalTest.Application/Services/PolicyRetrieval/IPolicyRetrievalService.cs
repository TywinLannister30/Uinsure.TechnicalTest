using Uinsure.TechnicalTest.Application.Dtos;

namespace Uinsure.TechnicalTest.Application.Services.PolicyRetrieval;

public interface IPolicyRetrievalService
{
    Task<PolicyDto?> GetPolicyAsync(Guid policyId);
}
