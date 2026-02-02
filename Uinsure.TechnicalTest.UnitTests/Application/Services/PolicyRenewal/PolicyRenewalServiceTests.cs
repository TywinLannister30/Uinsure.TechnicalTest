using Microsoft.Extensions.Options;
using Moq;
using Uinsure.TechnicalTest.Application.Configuration;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Services.PolicyRenewal;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Enums;
using Uinsure.TechnicalTest.Domain.Repository;
using Uinsure.TechnicalTest.UnitTests.Helpers;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyRenewal;

public class PolicyRenewalServiceTests
{
    private readonly Mock<IPolicyRepository> _mockPolicyRepository = new();

    private PolicyRenewalService CreateSut(IOptions<PolicySettings> policySettings) => new(_mockPolicyRepository.Object, policySettings);

    [Fact]
    public async Task RenewPolicyAsync_WhenPolicySettingsMissing_ThrowsArgumentNullException()
    {
        var policyId = Guid.NewGuid();
        var request = CreateRequest();

        var settings = Options.Create(new PolicySettings
        {
            RenewalWindowDays = 10,
            RenewalAutoPaymentMethodsAllowed = null
        });

        var sut = CreateSut(settings);

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.RenewPolicyAsync(policyId, request));

        Assert.Equal("PolicySettings configuration is missing.", ex.ParamName);
    }

    [Fact]
    public async Task RenewPolicyAsync_WhenPolicyNotFound_ReturnsNull()
    {
        var policyId = Guid.NewGuid();
        var request = CreateRequest();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policyId)).ReturnsAsync((Policy?)null);

        var sut = CreateSut(CreateSettings());

        var result = await sut.RenewPolicyAsync(policyId, request);

        Assert.Null(result);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RenewPolicyAsync_WhenAutoRenewAndPaymentNotAllowed_ReturnsPaymentMethodAllowedFalse()
    {
        var policy = PolicyHelpers.CreatePolicy(startDate: DateTimeOffset.UtcNow.AddYears(-1).AddDays(1));
        var request = CreateRequest(PaymentType.Card);

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var settings = CreateSettings(allowed: [PaymentType.DirectDebit]);

        var sut = CreateSut(settings);

        var result = await sut.RenewPolicyAsync(policy.Id, request);

        Assert.NotNull(result);
        Assert.False(result!.PaymentMethodAllowed);
        Assert.Null(result.Policy);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RenewPolicyAsync_WhenPolicyCancelled_ReturnsAlreadyCancelledTrue()
    {
        var policy = PolicyHelpers.CreatePolicy(isCancelled: true);
        var request = CreateRequest();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut(CreateSettings());

        var result = await sut.RenewPolicyAsync(policy.Id, request);

        Assert.NotNull(result);
        Assert.True(result!.AlreadyCancelled);
        Assert.Null(result.Policy);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RenewPolicyAsync_WhenOutsideRenewalWindow_ReturnsIsInRenewalWindowFalse()
    {
        var renewalWindowDays = 10;
        var startDate = DateTimeOffset.UtcNow.AddYears(-1).AddDays(renewalWindowDays + 2);
        var policy = PolicyHelpers.CreatePolicy(startDate: startDate);
        var request = CreateRequest();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut(CreateSettings(renewalWindowDays));

        var result = await sut.RenewPolicyAsync(policy.Id, request);

        Assert.NotNull(result);
        Assert.False(result!.IsInRenewalWindow);
        Assert.Null(result.Policy);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RenewPolicyAsync_WhenWithinRenewalWindow_RenewsPolicy_AddsPayment_Saves_ReturnsPolicy()
    {
        var startDate = DateTimeOffset.UtcNow.AddYears(-1).AddDays(1);
        var policy = PolicyHelpers.CreatePolicy(startDate: startDate);
        var request = CreateRequest(PaymentType.Card, amount: 50m);
        var originalEndDate = policy.EndDate;

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut(CreateSettings());

        var result = await sut.RenewPolicyAsync(policy.Id, request);

        Assert.NotNull(result);
        Assert.NotNull(result!.Policy);
        Assert.Equal(originalEndDate.AddYears(1), policy.EndDate);
        Assert.Equal(2, policy.Payments.Count);
        Assert.Equal(150m, policy.Amount);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    private static RenewPolicyRequestDto CreateRequest(PaymentType paymentType = PaymentType.Card, decimal amount = 12.34m)
    {
        return new RenewPolicyRequestDto
        {
            Payment = new PaymentDto
            {
                Reference = "reference",
                PaymentType = paymentType.ToString(),
                Amount = amount
            }
        };
    }

    private static IOptions<PolicySettings> CreateSettings(int renewalWindowDays = 10, List<PaymentType>? allowed = null)
    {
        return Options.Create(new PolicySettings
        {
            RenewalWindowDays = renewalWindowDays,
            RenewalAutoPaymentMethodsAllowed = allowed ?? [PaymentType.Card]
        });
    }
}
