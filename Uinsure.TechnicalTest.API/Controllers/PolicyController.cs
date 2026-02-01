using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService;
using Uinsure.TechnicalTest.Application.Services.PolicyCreationService;
using Uinsure.TechnicalTest.Application.Services.PolicyRetrievalService;

namespace Uinsure.TechnicalTest.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/policy")]
public class PolicyController(
    IPolicyCreationService policyCreationService, 
    IPolicyCancellationService policyCancellationService,
    IPolicyRetrievalService policyRetrievalService) : Controller
{
    IPolicyCancellationService _policyCancellationService = policyCancellationService;
    IPolicyCreationService _policyCreationService = policyCreationService;
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
}
