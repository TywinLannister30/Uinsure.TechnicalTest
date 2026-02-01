using RestSharp;
using System.Net;
using Uinsure.TechnicalTest.AcceptanceTests.Fixtures;
using Uinsure.TechnicalTest.AcceptanceTests.Helpers;
using Uinsure.TechnicalTest.Application.Dtos;

namespace Uinsure.TechnicalTest.AcceptanceTests.Tests.Controllers.Policy;

[Collection("Acceptance tests")]
public class MarkAsClaimTests(HttpClientFixture httpClientFixture)
{
    private readonly HttpClientFixture _httpClientFixture = httpClientFixture;

    [Fact]
    public async Task When_PolicyHasClaim_Expect_OkWithDetails()
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var markAsClaimRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/mark-as-claim", Method.Put);
        var markAsClaimResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(markAsClaimRequest);

        Assert.Equal(HttpStatusCode.OK, markAsClaimResponse.StatusCode);
        Assert.NotNull(markAsClaimResponse.Data);

        Assert.True(markAsClaimResponse.Data.HasClaims);
    }

    [Fact]
    public async Task When_CancellingANonExistentPolicy_Expect_NotFound()
    {
        var markAsClaimRequest = new RestRequest($"api/v1/policy/{Guid.NewGuid()}/mark-as-claim", Method.Put);
        var markAsClaimResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(markAsClaimRequest);

        Assert.Equal(HttpStatusCode.NotFound, markAsClaimResponse.StatusCode);
    }
}
