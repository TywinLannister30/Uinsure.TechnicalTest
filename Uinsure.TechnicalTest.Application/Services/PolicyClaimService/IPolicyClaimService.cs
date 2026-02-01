
using Uinsure.TechnicalTest.Application.Dtos;

namespace Uinsure.TechnicalTest.Application.Services.PolicyClaimService;

public interface IPolicyClaimService
{
    Task<PolicyDto?> MarkAsClaim(Guid policyId);
}
