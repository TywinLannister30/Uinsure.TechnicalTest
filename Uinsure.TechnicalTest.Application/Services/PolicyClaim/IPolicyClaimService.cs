using Uinsure.TechnicalTest.Application.Dtos;

namespace Uinsure.TechnicalTest.Application.Services.PolicyClaim;

public interface IPolicyClaimService
{
    Task<PolicyDto?> MarkAsClaimAsync(Guid policyId);
}
