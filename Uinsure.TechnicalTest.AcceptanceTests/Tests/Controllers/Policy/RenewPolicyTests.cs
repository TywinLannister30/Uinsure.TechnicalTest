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
public class RenewPolicyTests(HttpClientFixture httpClientFixture, DatabaseFixture databaseFixture)
{
    private readonly HttpClientFixture _httpClientFixture = httpClientFixture;
    private readonly DatabaseFixture _databaseFixture = databaseFixture;

    [Theory]
    [InlineData(30)]
    [InlineData(25)]
    [InlineData(10)]
    [InlineData(1)]
    public async Task When_PolicyRenewedWithinWindow_Expect_OkWithCorrectInformation(int numDays)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        // hack as dto vadiation prevents us adding policies in the past.
        var amendedEndDate = DateTimeOffset.UtcNow.AddDays(numDays);
        var amendedStartDate = amendedEndDate.AddYears(-1);

        await _databaseFixture.ExecuteAsync($"""
            UPDATE [UinsureTechnicalTest].[dbo].[Policies]
            SET StartDate = '{amendedStartDate.UtcDateTime:O}', EndDate = '{amendedEndDate.UtcDateTime:O}'
            WHERE Id = '{postResponse.Data.UniqueReference}'
        """);

        var renewRequestDto = new RenewPolicyRequestDto {
            Payment = new PaymentDto
            {
                Reference = "Reference",
                PaymentType = PaymentType.Card.ToString(),
                Amount = 199.99m
            }
        };
        var renewRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/renew", Method.Put).AddJsonBody(renewRequestDto);
        var renewResponse = await _httpClientFixture.Client.ExecuteAsync<RenewPolicyResponseDto>(renewRequest);

        Assert.Equal(HttpStatusCode.OK, renewResponse.StatusCode);
        Assert.NotNull(renewResponse.Data);

        Assert.Equal(amendedStartDate, renewResponse.Data.Policy.StartDate);
        Assert.Equal(amendedEndDate.AddYears(1), renewResponse.Data.Policy.EndDate);
        Assert.Equal(399.98m, renewResponse.Data.Policy.Amount);
        Assert.False(renewResponse.Data.Policy.HasClaims);
        Assert.Equal(dto.AutoRenew, renewResponse.Data.Policy.AutoRenew);

        var payments = renewResponse.Data.Policy.Payments;
        Assert.Equal(2, payments.Count);
    }

    [Theory]
    [InlineData(31)]
    [InlineData(40)]
    [InlineData(200)]
    [InlineData(365)]
    public async Task When_PolicyRenewedOutsideOfWindow_Expect_UnprocessableEntity(int numDays)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        // hack as dto vadiation prevents us adding policies in the past.
        var amendedEndDate = DateTimeOffset.UtcNow.AddDays(numDays);
        var amendedStartDate = amendedEndDate.AddYears(-1);

        await _databaseFixture.ExecuteAsync($"""
            UPDATE [UinsureTechnicalTest].[dbo].[Policies]
            SET StartDate = '{amendedStartDate.UtcDateTime:O}', EndDate = '{amendedEndDate.UtcDateTime:O}'
            WHERE Id = '{postResponse.Data.UniqueReference}'
        """);

        var renewRequestDto = new RenewPolicyRequestDto
        {
            Payment = new PaymentDto
            {
                Reference = "Reference",
                PaymentType = PaymentType.Card.ToString(),
                Amount = 199.99m
            }
        };
        var renewRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/renew", Method.Put).AddJsonBody(renewRequestDto);
        var renewResponse = await _httpClientFixture.Client.ExecuteAsync<RenewPolicyResponseDto>(renewRequest);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, renewResponse.StatusCode);
    }

    [Fact]
    public async Task When_PolicyRenewedButPolicyHasEnded_Expect_UnprocessableEntity()
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        // hack as dto vadiation prevents us adding policies in the past.
        var amendedEndDate = DateTimeOffset.UtcNow.AddDays(-1);
        var amendedStartDate = amendedEndDate.AddYears(-1);

        await _databaseFixture.ExecuteAsync($"""
            UPDATE [UinsureTechnicalTest].[dbo].[Policies]
            SET StartDate = '{amendedStartDate.UtcDateTime:O}', EndDate = '{amendedEndDate.UtcDateTime:O}'
            WHERE Id = '{postResponse.Data.UniqueReference}'
        """);

        var renewRequestDto = new RenewPolicyRequestDto
        {
            Payment = new PaymentDto
            {
                Reference = "Reference",
                PaymentType = PaymentType.Card.ToString(),
                Amount = 199.99m
            }
        };
        var renewRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/renew", Method.Put).AddJsonBody(renewRequestDto);
        var renewResponse = await _httpClientFixture.Client.ExecuteAsync<RenewPolicyResponseDto>(renewRequest);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, renewResponse.StatusCode);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(25)]
    [InlineData(10)]
    [InlineData(1)]
    public async Task When_PolicyRenewedWithinWindowButPolicyAlreadyCancelled_Expect_UnprocessableEntity(int numDays)
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

        await Task.Delay(500);

        // hack as dto vadiation prevents us adding policies in the past.
        var amendedEndDate = DateTimeOffset.UtcNow.AddDays(numDays);
        var amendedStartDate = amendedEndDate.AddYears(-1);

        await _databaseFixture.ExecuteAsync($"""
            UPDATE [UinsureTechnicalTest].[dbo].[Policies]
            SET StartDate = '{amendedStartDate.UtcDateTime:O}', EndDate = '{amendedEndDate.UtcDateTime:O}'
            WHERE Id = '{postResponse.Data.UniqueReference}'
        """);

        var renewRequestDto = new RenewPolicyRequestDto
        {
            Payment = new PaymentDto
            {
                Reference = "Reference",
                PaymentType = PaymentType.Card.ToString(),
                Amount = 199.99m
            }
        };
        var renewRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/renew", Method.Put).AddJsonBody(renewRequestDto);
        var renewResponse = await _httpClientFixture.Client.ExecuteAsync<RenewPolicyResponseDto>(renewRequest);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, renewResponse.StatusCode);
    }

    // This is Renew 3 a, and b. Only add a payment if policy is set to autorenew - not sure on the requirement here.
    // I have added a payment to the request assuming we would allow different payment methods between initial purchase and renewal.
    [Theory]
    [InlineData(30)]
    [InlineData(25)]
    [InlineData(10)]
    [InlineData(1)]
    public async Task When_PolicyRenewedWithinWindowButNotSetToAutoRenew_Expect_OkWithNoPaymentAdded(int numDays)
    {
        var dto = CreatePolicyRequestHelper.CreateValidRequest();
        dto.AutoRenew = false;

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        // hack as dto vadiation prevents us adding policies in the past.
        var amendedEndDate = DateTimeOffset.UtcNow.AddDays(numDays);
        var amendedStartDate = amendedEndDate.AddYears(-1);

        await _databaseFixture.ExecuteAsync($"""
            UPDATE [UinsureTechnicalTest].[dbo].[Policies]
            SET StartDate = '{amendedStartDate.UtcDateTime:O}', EndDate = '{amendedEndDate.UtcDateTime:O}'
            WHERE Id = '{postResponse.Data.UniqueReference}'
        """);

        var renewRequestDto = new RenewPolicyRequestDto
        {
            Payment = new PaymentDto
            {
                Reference = "Reference",
                PaymentType = PaymentType.Card.ToString(),
                Amount = 199.99m
            }
        };
        var renewRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}/renew", Method.Put).AddJsonBody(renewRequestDto);
        var renewResponse = await _httpClientFixture.Client.ExecuteAsync<RenewPolicyResponseDto>(renewRequest);

        Assert.Equal(HttpStatusCode.OK, renewResponse.StatusCode);
        Assert.NotNull(renewResponse.Data);

        Assert.Equal(amendedStartDate, renewResponse.Data.Policy.StartDate);
        Assert.Equal(amendedEndDate.AddYears(1), renewResponse.Data.Policy.EndDate);
        Assert.Equal(199.99m, renewResponse.Data.Policy.Amount);
        Assert.False(renewResponse.Data.Policy.HasClaims);
        Assert.Equal(dto.AutoRenew, renewResponse.Data.Policy.AutoRenew);

        var payments = renewResponse.Data.Policy.Payments;
        Assert.Single(payments);
    }

    [Fact]
    public async Task When_RenewingANonExistentPolicy_Expect_NotFound()
    {
        var renewRequestDto = new RenewPolicyRequestDto
        {
            Payment = new PaymentDto
            {
                Reference = "Reference",
                PaymentType = PaymentType.Card.ToString(),
                Amount = 199.99m
            }
        };
        var renewRequest = new RestRequest($"api/v1/policy/{Guid.NewGuid()}/renew", Method.Put).AddJsonBody(renewRequestDto);
        var renewResponse = await _httpClientFixture.Client.ExecuteAsync<RenewPolicyResponseDto>(renewRequest);

        Assert.Equal(HttpStatusCode.NotFound, renewResponse.StatusCode);
    }
}
