using Moq;
using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Application.Services.PolicyCreation;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Enums;
using Uinsure.TechnicalTest.Domain.Repository;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyCreation;

public class PolicyCreationServiceTests
{
    private readonly Mock<IPolicyRepository> _mockPolicyRepository = new();

    private PolicyCreationService CreateSut() => new(_mockPolicyRepository.Object);

    [Fact]
    public async Task CreatePolicyAsync_NoAnomolies_ReturnsPolicy()
    {
        var request = CreateValidRequest();

        _mockPolicyRepository.Setup(r => r.SaveAsync(It.IsAny<Policy>(), default)).Verifiable();

        var sut = CreateSut();

        var result = await sut.CreatePolicyAsync(request);

        Assert.NotNull(result);

        _mockPolicyRepository.Verify(r => r.SaveAsync(It.IsAny<Policy>(), default), Times.Once);
    }

    private static CreatePolicyRequestDto CreateValidRequest()
    {
        return new()
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
}
