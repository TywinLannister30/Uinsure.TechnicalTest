using RestSharp;
using System.Net;
using Uinsure.TechnicalTest.AcceptanceTests.Fixtures;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.AcceptanceTests.Tests.Controllers.Policy;

[Collection("Acceptance tests")]
public class GetPolicyTests(HttpClientFixture httpClientFixture, DatabaseFixture databaseFixture)
{
    private readonly HttpClientFixture _httpClientFixture = httpClientFixture;
    private readonly DatabaseFixture _databaseFixture = databaseFixture;

    [Fact]
    public async Task When_RetreivingExistingPolicy_Expect_OkResponseAndCorrectInformationReturned()
    {
        var dto = BuildValidRequest();

        var postRequest = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var postResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(postRequest);

        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.NotNull(postResponse.Data);

        await Task.Delay(500);

        var getRequest = new RestRequest($"api/v1/policy/{postResponse.Data.UniqueReference}", Method.Get);
        var getResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getResponse.Data);

        Assert.Equal(dto.StartDate, getResponse.Data.StartDate);
        Assert.Equal(dto.StartDate.AddYears(1), getResponse.Data.EndDate);
        Assert.Equal(dto.InsuranceType, getResponse.Data.InsuranceType);
        Assert.Equal(dto.Payment.Amount, getResponse.Data.Amount);
        Assert.False(getResponse.Data.HasClaims);
        Assert.Equal(dto.AutoRenew, getResponse.Data.AutoRenew);
        
        Assert.Single(getResponse.Data.Policyholders);
        Assert.Equal(dto.Policyholders.First().FirstName, getResponse.Data.Policyholders.First().FirstName);
        Assert.Equal(dto.Policyholders.First().LastName, getResponse.Data.Policyholders.First().LastName);
        Assert.Equal(dto.Policyholders.First().DateOfBirth.Date, getResponse.Data.Policyholders.First().DateOfBirth.Date);

        Assert.Single(getResponse.Data.Payments);
        Assert.Equal(dto.Payment.Reference, getResponse.Data.Payments.First().Reference);
        Assert.Equal(dto.Payment.Amount, getResponse.Data.Payments.First().Amount);
        Assert.Equal(dto.Payment.PaymentType, getResponse.Data.Payments.First().PaymentType);

        Assert.NotNull(getResponse.Data.Property);
        Assert.Equal(dto.Property.AddressLine1, getResponse.Data.Property.AddressLine1);
        Assert.Equal(dto.Property.AddressLine2, getResponse.Data.Property.AddressLine2);
        Assert.Equal(dto.Property.AddressLine3, getResponse.Data.Property.AddressLine3);
        Assert.Equal(dto.Property.Postcode, getResponse.Data.Property.Postcode);
    }

    [Fact]
    public async Task When_RetreivingNonExistingPolicy_Expect_NotFound()
    {
        var getRequest = new RestRequest($"api/v1/policy/{Guid.NewGuid()}", Method.Get);
        var getResponse = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(getRequest);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static CreatePolicyRequestDto BuildValidRequest() => new()
    {
        InsuranceType = InsuranceType.Household.ToString(),
        StartDate = DateTimeOffset.UtcNow,
        AutoRenew = true,
        Policyholders =
        [
            new PolicyholderDto
            {
                FirstName = "FirstName",
                LastName = "LastName",
                DateOfBirth = DateTimeOffset.UtcNow.AddYears(-20),
            }
        ],
        Property = new PropertyDto
        {
            AddressLine1 = "AddressLine1",
            AddressLine2 = "AddressLine2",
            AddressLine3 = "AddressLine3",
            Postcode = "Postcode"
        },
        Payment = new PaymentDto
        {
            Reference = "Reference",
            PaymentType = PaymentType.Card.ToString(),
            Amount = 199.99m
        }
    };
}
