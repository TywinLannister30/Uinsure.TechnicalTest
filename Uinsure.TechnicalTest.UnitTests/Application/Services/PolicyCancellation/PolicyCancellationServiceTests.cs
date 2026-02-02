using Moq;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.Factories;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Enums;
using Uinsure.TechnicalTest.Domain.Repository;
using Uinsure.TechnicalTest.UnitTests.Helpers;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyCancellation;

public class PolicyCancellationServiceTests
{
    private readonly Mock<IPolicyRepository> _mockPolicyRepository = new();
    private readonly Mock<IRefundProcessorFactory> _mockRefundProcessorFactory = new();

    private PolicyCancellationService CreateSut() => new(_mockPolicyRepository.Object, _mockRefundProcessorFactory.Object);

    [Fact]
    public async Task CancelPolicyAsync_WhenPolicyNotFound_ReturnsNull()
    {
        var policyId = Guid.NewGuid();

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policyId)).ReturnsAsync((Policy?)null);

        var sut = CreateSut();

        var result = await sut.CancelPolicyAsync(
            policyId,
            new CancelPolicyRequestDto { CancellationDate = DateTimeOffset.UtcNow });

        Assert.Null(result);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockRefundProcessorFactory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelPolicyAsync_WhenAlreadyCancelled_ReturnsAlreadyCancelledTrue_DoesNotSave()
    {
        var policy = PolicyHelpers.CreatePolicy(isCancelled: true);

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut();

        var result = await sut.CancelPolicyAsync(
            policy.Id,
            new CancelPolicyRequestDto { CancellationDate = DateTimeOffset.UtcNow });

        Assert.NotNull(result);
        Assert.True(result!.AlreadyCancelled);
        Assert.Null(result.Policy);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockRefundProcessorFactory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelPolicyAsync_WhenHasClaims_ActionRefundTrue_CancelsAndSaves_RefundZero_ReturnsPolicy()
    {
        var cancellationDate = new DateTimeOffset(2030, 1, 10, 0, 0, 0, TimeSpan.Zero);

        var policy = PolicyHelpers.CreatePolicy(hasClaims: true);

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut();

        var result = await sut.CancelPolicyAsync(
            policy.Id,
            new CancelPolicyRequestDto { CancellationDate = cancellationDate },
            actionRefund: true);

        Assert.NotNull(result);
        Assert.False(result!.AlreadyCancelled);
        Assert.Equal(0, result.RefundAmount);
        Assert.NotNull(result.Policy);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockRefundProcessorFactory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelPolicyAsync_WhenHasClaims_ActionRefundFalse_ReturnsRefundZero_DoesNotSave()
    {
        var policy = PolicyHelpers.CreatePolicy(hasClaims: true);

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        var sut = CreateSut();

        var result = await sut.CancelPolicyAsync(
            policy.Id,
            new CancelPolicyRequestDto { CancellationDate = DateTimeOffset.UtcNow },
            actionRefund: false);

        Assert.NotNull(result);
        Assert.Equal(0, result!.RefundAmount);
        Assert.Null(result.Policy);
        
        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockRefundProcessorFactory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelPolicyAsync_WhenNoClaims_AndFactoryReturnsNull_ThrowsInvalidOperationException()
    {
        var start = new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cancel = new DateTimeOffset(2029, 1, 10, 0, 0, 0, TimeSpan.Zero);

        var policy = PolicyHelpers.CreatePolicy(startDate: start);

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        _mockRefundProcessorFactory.Setup(f => f.GetRefundProcessor(start, cancel)).Returns((IRefundProcessor?)null);

        var sut = CreateSut();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CancelPolicyAsync(policy.Id, new CancelPolicyRequestDto { CancellationDate = cancel }, actionRefund: true)
        );

        Assert.Contains("No refund processor found", ex.Message);

        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CancelPolicyAsync_WhenNoClaims_ActionRefundTrue_ProcessesRefund_Cancels_AddsPayment_Saves_ReturnsPolicyAndAbsRefund()
    {
        var start = new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cancel = new DateTimeOffset(2029, 1, 11, 0, 0, 0, TimeSpan.Zero);

        var policy = PolicyHelpers.CreatePolicy(startDate: start);
        var refundPayment = PolicyHelpers.CreatePayment(12.34m, TransactionType.Refund, policy.Id);

        var processor = new Mock<IRefundProcessor>();
        processor.Setup(p => p.Process(policy, cancel)).Returns(refundPayment);

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        _mockRefundProcessorFactory.Setup(f => f.GetRefundProcessor(start, cancel)).Returns(processor.Object);

        var sut = CreateSut();

        var result = await sut.CancelPolicyAsync(
            policy.Id,
            new CancelPolicyRequestDto { CancellationDate = cancel },
            actionRefund: true);

        Assert.NotNull(result);
        Assert.NotNull(result!.Policy);
        Assert.Equal(12.34m, result.RefundAmount);

        processor.Verify(p => p.Process(policy, cancel), Times.Once);
        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelPolicyAsync_WhenNoClaims_ActionRefundFalse_ProcessesRefund_ReturnsAbsRefund_DoesNotSave()
    {
        var start = new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cancel = new DateTimeOffset(2029, 1, 11, 0, 0, 0, TimeSpan.Zero);

        var policy = PolicyHelpers.CreatePolicy(startDate: start);
        var refundPayment = PolicyHelpers.CreatePayment(-50m, TransactionType.Refund, policy.Id);

        var processor = new Mock<IRefundProcessor>();
        processor.Setup(p => p.Process(policy, cancel)).Returns(refundPayment);

        _mockPolicyRepository.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);

        _mockRefundProcessorFactory.Setup(f => f.GetRefundProcessor(start, cancel)).Returns(processor.Object);

        var sut = CreateSut();

        var result = await sut.CancelPolicyAsync(
            policy.Id,
            new CancelPolicyRequestDto { CancellationDate = cancel },
            actionRefund: false);

        Assert.NotNull(result);
        Assert.Equal(50m, result!.RefundAmount);
        Assert.Null(result.Policy);

        processor.Verify(p => p.Process(policy, cancel), Times.Once);
        _mockPolicyRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
