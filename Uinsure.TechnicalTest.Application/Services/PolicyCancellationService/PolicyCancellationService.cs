using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Application.Mappers;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.Factories;
using Uinsure.TechnicalTest.Domain.Enums;
using Uinsure.TechnicalTest.Domain.Repository;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellationService;

public class PolicyCancellationService(IPolicyRepository policyRepository, IRefundProcessorFactory refundProcessorFactory) : IPolicyCancellationService
{
    private readonly IPolicyRepository _policyRepository = policyRepository;
    private readonly IRefundProcessorFactory _refundProcessorFactory = refundProcessorFactory;

    public async Task<CancelPolicyResponseDto?> CancelPolicyAsync(Guid policyId, CancelPolicyRequestDto request)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId);

        if (policy is null)
            return null;

        if (policy.IsCancelled())
            return new CancelPolicyResponseDto { AlreadyCancelled = true };

        var refundProcessor = _refundProcessorFactory.GetRefundProcessor(policy.StartDate, request.CancellationDate);

        if (refundProcessor is null)
            throw new InvalidOperationException($"No refund processor found for policy starting {policy.StartDate} and cancellation date {request.CancellationDate}.");

        policy.Cancel(request.CancellationDate);

        var refund = refundProcessor.Process(policy);

        policy.AddPayment(refund);

        await _policyRepository.SaveChangesAsync();

        return new CancelPolicyResponseDto
        {
            Policy = policy.ToDto(),
            RefundAmount = Math.Abs(refund.Amount),
        };
    }
}
