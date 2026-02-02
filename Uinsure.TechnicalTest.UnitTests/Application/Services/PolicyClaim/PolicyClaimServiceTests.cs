using Moq;
using Uinsure.TechnicalTest.Application.Services.PolicyClaim;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Repository;
using Uinsure.TechnicalTest.UnitTests.Helpers;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyClaim;

public class PolicyClaimServiceTests
{
    private readonly Mock<IPolicyRepository> _mockPolicyRepository = new();

    private PolicyClaimService CreateSut() => new(_mockPolicyRepository.Object);

    [Fact]
    public async Task MarkAsClaimAsync_WhenPolicyNotFound_ReturnsNull()
    {
        var policyId = Guid.NewGuid();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policyId)).ReturnsAsync((Policy?)null);

        var sut = CreateSut();

        var result = await sut.MarkAsClaimAsync(policyId);

        Assert.Null(result);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CancelPolicyAsync_WhenPolicyIsFound_()
    {
        var policy = PolicyHelpers.CreatePolicy();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut();

        var result = await sut.MarkAsClaimAsync(policy.Id);

        Assert.NotNull(result);
        Assert.True(policy.HasClaims);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
