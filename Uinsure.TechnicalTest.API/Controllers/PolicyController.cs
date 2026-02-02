using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation;
using Uinsure.TechnicalTest.Application.Services.PolicyClaim;
using Uinsure.TechnicalTest.Application.Services.PolicyCreation;
using Uinsure.TechnicalTest.Application.Services.PolicyRenewal;
using Uinsure.TechnicalTest.Application.Services.PolicyRetrieval;

namespace Uinsure.TechnicalTest.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/policy")]
public class PolicyController(
    IPolicyCreationService policyCreationService, 
    IPolicyCancellationService policyCancellationService,
    IPolicyClaimService policyClaimService,
    IPolicyRenewalService policyRenewalService,
    IPolicyRetrievalService policyRetrievalService) : Controller
{
    IPolicyCancellationService _policyCancellationService = policyCancellationService;
    IPolicyCreationService _policyCreationService = policyCreationService;
    IPolicyClaimService _policyClaimService = policyClaimService;
    IPolicyRenewalService _policyRenewalService = policyRenewalService;
    IPolicyRetrievalService _policyRetrievalService = policyRetrievalService;

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PolicyDto))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IActionResult))]
    public async Task<IActionResult> Post(CreatePolicyRequestDto request)
    {
        var result = await _policyCreationService.CreatePolicyAsync(request);

        return Ok(result);
    }

    [HttpGet("{policyId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PolicyDto))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(IActionResult))]
    public async Task<IActionResult> Get(Guid policyId)
    {
        var result = await _policyRetrievalService.GetPolicyAsync(policyId);

        if (result is null)
            return NotFound($"Policy with id {policyId} does not exist.");

        return Ok(result);
    }

    [HttpPut("{policyId:guid}/cancel")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CancelPolicyResponseDto))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(IActionResult))]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity, Type = typeof(IActionResult))]
    public async Task<IActionResult> Cancel(Guid policyId, CancelPolicyRequestDto request)
    {
        var result = await _policyCancellationService.CancelPolicyAsync(policyId, request);

        if (result is null)
            return NotFound($"Policy with id {policyId} does not exist.");

        if (result.AlreadyCancelled)
            return UnprocessableEntity($"Policy with id {policyId} is already cancelled.");

        return Ok(result);
    }

    // This is to enable COULD 1 - Calculate the cost to cancel a policy before the policy has actually been cancelled.
    // Making this a POST as we could store and retrieve these.
    [HttpPost("{policyId:guid}/cancellation-quote")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CancellationQuoteResponseDto))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(IActionResult))]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity, Type = typeof(IActionResult))]
    public async Task<IActionResult> CancellationQuote(Guid policyId, CancelPolicyRequestDto request)
    {
        var result = await _policyCancellationService.CancelPolicyAsync(policyId, request, actionRefund: false);

        if (result is null)
            return NotFound($"Policy with id {policyId} does not exist.");

        if (result.AlreadyCancelled)
            return UnprocessableEntity($"Policy with id {policyId} is already cancelled.");

        return Ok(result);
    }

    [HttpPut("{policyId:guid}/renew")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RenewPolicyResponseDto))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(IActionResult))]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity, Type = typeof(IActionResult))]
    public async Task<IActionResult> Renew(Guid policyId, RenewPolicyRequestDto request)
    {
        var result = await _policyRenewalService.RenewPolicyAsync(policyId, request);

        if (result is null)
            return NotFound($"Policy with id {policyId} does not exist.");

        if (result.AlreadyCancelled)
            return UnprocessableEntity($"Policy with id {policyId} is already cancelled and cannot be renewed.");

        if (!result.PaymentMethodAllowed)
            return UnprocessableEntity($"{request.Payment.PaymentType} cannot be used to pay for auto renewals.");

        if (!result.IsInRenewalWindow)
            return UnprocessableEntity($"Policy with id {policyId} is not inside the renewal window.");

        if (result.PolicyEnded)
            return UnprocessableEntity($"Policy with id {policyId} has ended and cannot be renewed.");

        return Ok(result);
    }

    // To allow Could 2. - Not issue a refund if the policy has had a claim made against it.
    [HttpPut("{policyId:guid}/mark-as-claim")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PolicyDto))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(IActionResult))]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity, Type = typeof(IActionResult))]
    public async Task<IActionResult> MarkAsClaim(Guid policyId)
    {
        var result = await _policyClaimService.MarkAsClaimAsync(policyId);

        if (result is null)
            return NotFound($"Policy with id {policyId} does not exist.");

        return Ok(result);
    }
}
