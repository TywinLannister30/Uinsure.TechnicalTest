using RestSharp;
using System.Net;
using Uinsure.TechnicalTest.AcceptanceTests.Fixtures;
using Uinsure.TechnicalTest.AcceptanceTests.Helpers;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;

namespace Uinsure.TechnicalTest.AcceptanceTests.Tests.Controllers.Policy;

[Collection("Acceptance tests")]
public class CancellationQuoteTests(HttpClientFixture httpClientFixture)
{
    private readonly HttpClientFixture _httpClientFixture = httpClientFixture;

    [Fact]
    public async Task When_PolicyCancelledBeforeItStarts_Expect_OkWithFullRefundQuoted()
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(-1) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancellation-quote", Method.Post).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancellationQuoteResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.Equal(dto.Payment.Amount, cancelResponse.Data.RefundAmount);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(13)]
    [InlineData(14)]
    public async Task When_PolicyCancelledBeforeCooldownPeroid_Expect_OkWithFullRefundQuoted(int numDays)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(numDays) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancellation-quote", Method.Post).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancellationQuoteResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.Equal(199.99m, cancelResponse.Data.RefundAmount);
    }

    [Theory]
    [InlineData(15, 191.77)]
    [InlineData(30, 183.55)]
    [InlineData(200, 90.41)]
    [InlineData(364, 0.55)]
    public async Task When_PolicyCancelledAfterCooldownPeroid_Expect_OkWithProRataRefundQuoted(int numDays, decimal expectedRefund)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(numDays) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancellation-quote", Method.Post).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancellationQuoteResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.True(cancelResponse.Data.RefundAmount < 199.99m);
        Assert.Equal(expectedRefund, cancelResponse.Data.RefundAmount);
    }

    [Theory]
    [InlineData(366)]
    [InlineData(400)]
    public async Task When_PolicyCancelledAfterPolicyEnded_Expect_NoRefundQuoted(int numDays)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(numDays) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancellation-quote", Method.Post).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancellationQuoteResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.Equal(0, cancelResponse.Data.RefundAmount);
    }

    [Fact]
    public async Task When_CancellingANonExistentPolicy_Expect_NotFound()
    {
        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = DateTimeOffset.UtcNow };
        var cancelRequest = new RestRequest($"api/v1/policy/{Guid.NewGuid()}/cancellation-quote", Method.Post).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancellationQuoteResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.NotFound, cancelResponse.StatusCode);
    }

    [Fact]
    public async Task When_CancellingAnAlreadyCancelledPolicy_Expect_UnprocessableEntity()
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(1) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        await Task.Delay(500);

        var cancellationQuoteDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(1) };
        var cancellationQuoteRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancellation-quote", Method.Post).AddJsonBody(cancellationQuoteDto);
        var cancellationQuoteResponse = await _httpClientFixture.Client.ExecuteAsync<CancellationQuoteResponseDto>(cancellationQuoteRequest);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, cancellationQuoteResponse.StatusCode);
    }
}
