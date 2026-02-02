using Moq;
using Uinsure.TechnicalTest.Application.Services.PolicyRetrieval;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Repository;
using Uinsure.TechnicalTest.UnitTests.Helpers;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyRetrieval;

public class PolicyRetrievalServiceTests
{
    private readonly Mock<IPolicyRepository> _mockPolicyRepository = new();

    private PolicyRetrievalService CreateSut() => new(_mockPolicyRepository.Object);

    [Fact]
    public async Task GetPolicyAsync_WhenPolicyNotFound_ReturnsNull()
    {
        var policyId = Guid.NewGuid();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policyId)).ReturnsAsync((Policy?)null);

        var sut = CreateSut();

        var result = await sut.GetPolicyAsync(policyId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPolicyAsync_WhenPolicyIsFound_ReturnsPolicyDto()
    {
        var policy = PolicyHelpers.CreatePolicy();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut();

        var result = await sut.GetPolicyAsync(policy.Id);

        Assert.NotNull(result);
        Assert.Equal(policy.Id, result!.UniqueReference);
        Assert.Equal(policy.Amount, result.Amount);
        Assert.Equal(policy.AutoRenew, result.AutoRenew);
    }
}
