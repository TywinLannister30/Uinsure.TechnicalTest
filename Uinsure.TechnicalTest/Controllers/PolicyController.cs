using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Application.Services.PolicyService;

namespace Uinsure.TechnicalTest.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/policy")]
public class PolicyController(IPolicyService policyService) : Controller
{
    IPolicyService _policyService = policyService;

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CreatePolicyResponseDto))]
    public async Task<IActionResult> Post(CreatePolicyRequestDto request)
    {
        var result = await _policyService.CreatePolicyAsync(request);

        return Ok(result);
    }
}
