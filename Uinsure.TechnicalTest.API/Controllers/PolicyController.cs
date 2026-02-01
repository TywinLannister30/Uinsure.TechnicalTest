using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Services.PolicyService;

namespace Uinsure.TechnicalTest.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/policy")]
public class PolicyController(IPolicyService policyService) : Controller
{
    IPolicyService _policyService = policyService;

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PolicyDto))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IActionResult))]
    public async Task<IActionResult> Post(CreatePolicyRequestDto request)
    {
        var result = await _policyService.CreatePolicyAsync(request);

        return Ok(result);
    }

    [HttpGet("policyId")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PolicyDto))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IActionResult))]
    public async Task<IActionResult> Get(Guid policyId)
    {
        var result = await _policyService.GetPolicyAsync(policyId);

        if (result is null)
            return NotFound($"Policy with id {policyId} does not exist.");

        return Ok(result);
    }
}
