using RestSharp;
using System.Net;
using Uinsure.TechnicalTest.AcceptanceTests.Fixtures;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.AcceptanceTests.Tests.Controllers.Policy;

[Collection("Acceptance tests")]
public class PostPolicyTests(HttpClientFixture httpClientFixture, DatabaseFixture databaseFixture)
{
    private readonly HttpClientFixture _httpClientFixture = httpClientFixture;
    private readonly DatabaseFixture _databaseFixture = databaseFixture;

    [Fact]
    public async Task When_PolicyCreatedSuccessfully_Expect_OkResponseAndCorrectInformationStored()
    {
        var dto = BuildValidRequest();

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(dto.StartDate, response.Data.StartDate);
        Assert.Equal(dto.StartDate.AddYears(1), response.Data.EndDate);
        Assert.Equal(dto.InsuranceType, response.Data.InsuranceType);
        Assert.Equal(dto.Payment.Amount, response.Data.Amount);
        Assert.False(response.Data.HasClaims);
        Assert.Equal(dto.AutoRenew, response.Data.AutoRenew);
        
        Assert.Single(response.Data.Policyholders);
        Assert.Equal(dto.Policyholders.First().FirstName, response.Data.Policyholders.First().FirstName);
        Assert.Equal(dto.Policyholders.First().LastName, response.Data.Policyholders.First().LastName);
        Assert.Equal(dto.Policyholders.First().DateOfBirth.Date, response.Data.Policyholders.First().DateOfBirth.Date);

        Assert.Single(response.Data.Payments);
        Assert.Equal(dto.Payment.Reference, response.Data.Payments.First().Reference);
        Assert.Equal(dto.Payment.Amount, response.Data.Payments.First().Amount);
        Assert.Equal(dto.Payment.PaymentType, response.Data.Payments.First().PaymentType);

        Assert.NotNull(response.Data.Property);
        Assert.Equal(dto.Property.AddressLine1, response.Data.Property.AddressLine1);
        Assert.Equal(dto.Property.AddressLine2, response.Data.Property.AddressLine2);
        Assert.Equal(dto.Property.AddressLine3, response.Data.Property.AddressLine3);
        Assert.Equal(dto.Property.Postcode, response.Data.Property.Postcode);
    }

    [Fact]
    public async Task When_PolicyCreatedWithStartDateInPast_Expect_BadRequest()
    {
        var dto = BuildValidRequest();
        dto.StartDate = DateTime.UtcNow.AddDays(-1);

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task When_PolicyCreatedWithStartDate60DaysInTheFuture_Expect_OK()
    {
        var dto = BuildValidRequest();
        dto.StartDate = DateTime.UtcNow.AddDays(60);

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(61)]
    [InlineData(62)]
    [InlineData(100)]
    public async Task When_PolicyCreatedWithStartDateMoreThan60DaysInTheFuture_Expect_OK(int numDays)
    {
        var dto = BuildValidRequest();
        dto.StartDate = DateTime.UtcNow.AddDays(numDays);

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task When_PolicyCreatedWithCorrectNumberOfPolicyholders_Expect_OK(int numPolicyholders)
    {
        var dto = BuildValidRequest();
        dto.Policyholders = [];

        for (int i = 0; i < numPolicyholders; i++)
        {
            dto.Policyholders.Add(new PolicyholderDto
            {
                FirstName = i.ToString(),
                LastName = i.ToString(),
                DateOfBirth = DateTimeOffset.UtcNow.AddYears(-20)
            });
        }

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(6)]
    public async Task When_PolicyCreatedWithIncorrectCorrectNumberOfPolicyholders_Expect_BadRequest(int numPolicyholders)
    {
        var dto = BuildValidRequest();
        dto.Policyholders = [];

        for (int i = 0; i < numPolicyholders; i++)
        {
            dto.Policyholders.Add(new PolicyholderDto
            {
                FirstName = i.ToString(),
                LastName = i.ToString(),
                DateOfBirth = DateTimeOffset.UtcNow.AddYears(-20)
            });
        }

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task When_PolicyCreatedWithPolicyholderThatIsUnder16AtStartTimeOfPolicy_Expect_BadRequest()
    {
        var dto = BuildValidRequest();
        dto.Policyholders.First().DateOfBirth = dto.StartDate.AddYears(-16).AddDays(1);

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task When_PolicyCreatedWithPolicyholderThatIs16AtStartTimeOfPolicy_Expect_OK()
    {
        var dto = BuildValidRequest();
        dto.Policyholders.First().DateOfBirth = dto.StartDate.AddYears(-16);

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_PolicyCreatedWithPolicyholderThatIsUnder16AtStartTimeOfPolicyButOtherPolicyHolderIsOldEnough_Expect_BadRequest()
    {
        var dto = BuildValidRequest();
        dto.Policyholders =
        [
            new PolicyholderDto
            {
                FirstName = "Old",
                LastName = "Enough",
                DateOfBirth = dto.StartDate.AddYears(-20)
            },
            new PolicyholderDto
            {
                FirstName = "Too",
                LastName = "Young",
                DateOfBirth = dto.StartDate.AddYears(-16).AddDays(1)
            },
        ];

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    //[InlineData(null)]
    [InlineData("")]
    public async Task When_PolicyCreatedWithPropertyMissingAddressLine1_Expect_BadRequest(string? input)
    {
        var dto = BuildValidRequest();
        dto.Property.AddressLine1 = input;

        var request = new RestRequest($"api/v1/policy", Method.Post).AddJsonBody(dto);
        var response = await _httpClientFixture.Client.ExecuteAsync<PolicyDto>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
