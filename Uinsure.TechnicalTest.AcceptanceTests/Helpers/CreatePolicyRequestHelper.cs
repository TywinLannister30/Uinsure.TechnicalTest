using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Application.Dtos.Api.Request;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.AcceptanceTests.Helpers;

public static class CreatePolicyRequestHelper
{
    public static CreatePolicyRequestDto CreateValidRequest()
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
