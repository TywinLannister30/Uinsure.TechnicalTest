using RestSharp;
using System.Net;
using Uinsure.TechnicalTest.AcceptanceTests.Fixtures;
using Uinsure.TechnicalTest.AcceptanceTests.Helpers;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Dtos.Api.Response;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.AcceptanceTests.Tests.Controllers.Policy;

[Collection("Acceptance tests")]
public class CancelPolicyTests(HttpClientFixture httpClientFixture)
{
    private readonly HttpClientFixture _httpClientFixture = httpClientFixture;

    [Fact]
    public async Task When_PolicyCancelledBeforeItStarts_Expect_OkWithFullRefundAndDetails()
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(-1) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.Equal(dto.StartDate, cancelResponse.Data.Policy.StartDate);
        Assert.Equal(dto.StartDate.AddYears(1), cancelResponse.Data.Policy.EndDate);
        Assert.Equal(dto.InsuranceType, cancelResponse.Data.Policy.InsuranceType);
        Assert.Equal(0, cancelResponse.Data.Policy.Amount);
        Assert.False(cancelResponse.Data.Policy.HasClaims);
        Assert.Equal(dto.AutoRenew, cancelResponse.Data.Policy.AutoRenew);

        Assert.Single(cancelResponse.Data.Policy.Policyholders);
        Assert.Equal(dto.Policyholders.First().FirstName, cancelResponse.Data.Policy.Policyholders.First().FirstName);
        Assert.Equal(dto.Policyholders.First().LastName, cancelResponse.Data.Policy.Policyholders.First().LastName);
        Assert.Equal(dto.Policyholders.First().DateOfBirth.Date, cancelResponse.Data.Policy.Policyholders.First().DateOfBirth.Date);

        var payments = cancelResponse.Data.Policy.Payments;
        Assert.Equal(2, payments.Count);
        Assert.Single(payments, x => x.TransactionType == TransactionType.Payment.ToString());
        Assert.Single(payments, x => x.TransactionType == TransactionType.Refund.ToString());

        Assert.NotNull(cancelResponse.Data.Policy.Property);
        Assert.Equal(dto.Property.AddressLine1, cancelResponse.Data.Policy.Property.AddressLine1);
        Assert.Equal(dto.Property.AddressLine2, cancelResponse.Data.Policy.Property.AddressLine2);
        Assert.Equal(dto.Property.AddressLine3, cancelResponse.Data.Policy.Property.AddressLine3);
        Assert.Equal(dto.Property.Postcode, cancelResponse.Data.Policy.Property.Postcode);

        Assert.Equal(dto.Payment.Amount, cancelResponse.Data.RefundAmount);
    }

    [Fact]
    public async Task When_PolicyHasClaims_Expect_OkWithNoRefundAndCancellation()
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

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(1) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);
        Assert.NotNull(cancelResponse.Data.Policy);

        Assert.True(cancelResponse.Data.Policy.HasClaims);
        Assert.Equal(0, cancelResponse.Data.RefundAmount);
        Assert.Single(cancelResponse.Data.Policy.Payments);
        Assert.Single(cancelResponse.Data.Policy.Payments, x => x.TransactionType == TransactionType.Payment.ToString());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(13)]
    [InlineData(14)]
    public async Task When_PolicyCancelledBeforeCooldownPeroid_Expect_OkWithFullRefund(int numDays)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(numDays) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.Equal(199.99m, cancelResponse.Data.RefundAmount);
    }

    [Theory]
    [InlineData(15, 191.77)]
    [InlineData(30, 183.55)]
    [InlineData(200, 90.41)]
    [InlineData(364, 0.55)]
    public async Task When_PolicyCancelledAfterCooldownPeroid_Expect_OkWithProRataRefund(int numDays, decimal expectedRefund)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(numDays) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.True(cancelResponse.Data.RefundAmount < 199.99m);
        Assert.Equal(expectedRefund, cancelResponse.Data.RefundAmount);
    }

    [Theory]
    [InlineData(366)]
    [InlineData(400)]
    public async Task When_PolicyCancelledAfterPolicyEnded_Expect_NoRefund(int numDays)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(numDays) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.Equal(0, cancelResponse.Data.RefundAmount);
    }

    [Fact]
    public async Task When_CancellingANonExistentPolicy_Expect_NotFound()
    {
        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = DateTimeOffset.UtcNow };
        var cancelRequest = new RestRequest($"api/v1/policy/{Guid.NewGuid()}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

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

        var firstCancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(1) };
        var firstCancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(firstCancellationDto);
        var firstCancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(firstCancelRequest);

        Assert.Equal(HttpStatusCode.OK, firstCancelResponse.StatusCode);
        Assert.NotNull(firstCancelResponse.Data);

        await Task.Delay(500);

        var secondCancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(1) };
        var secondCancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(secondCancellationDto);
        var secondCancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(secondCancelRequest);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, secondCancelResponse.StatusCode);
    }

    [Fact]
    public async Task When_PolicyCancelledBeforeItStartsButPolicyHasAClaim_Expect_OkWithNoRefundAndDetails()
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

        await Task.Delay(500);

        var cancellationDto = new CancelPolicyRequestDto { CancellationDate = postResponse.Data.StartDate.AddDays(-1) };
        var cancelRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/cancel", Method.Put).AddJsonBody(cancellationDto);
        var cancelResponse = await _httpClientFixture.Client.ExecuteAsync<CancelPolicyResponseDto>(cancelRequest);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        Assert.NotNull(cancelResponse.Data);

        Assert.Equal(199.99m, cancelResponse.Data.Policy.Amount);
        Assert.True(cancelResponse.Data.Policy.HasClaims);

        var payments = cancelResponse.Data.Policy.Payments;
        Assert.Single(payments);
        Assert.Equal(0, cancelResponse.Data.RefundAmount);
    }
}
