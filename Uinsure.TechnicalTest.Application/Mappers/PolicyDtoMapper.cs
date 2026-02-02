using Uinsure.TechnicalTest.Application.Dtos;
using Uinsure.TechnicalTest.Domain.Aggregates;

namespace Uinsure.TechnicalTest.Application.Mappers;

public static class PolicyDtoMapper
{
    public static PolicyDto ToDto(this Policy policy)
    {
        return new PolicyDto
        {
            UniqueReference = policy.Id,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate,
            State = policy.State.ToString(),
            InsuranceType = policy.InsuranceType.ToString(),
            Amount = policy.Amount,
            HasClaims = policy.HasClaims,
            AutoRenew = policy.AutoRenew,
            Policyholders = [.. policy.Policyholders
                .Select(ph => new PolicyholderDto
                {
                    FirstName = ph.FirstName ?? string.Empty,
                    LastName = ph.LastName ?? string.Empty,
                    DateOfBirth = ph.DateOfBirth.UtcDateTime.Date
                })],
            Payments = [.. policy.Payments
                .Select(p => new PaymentDto
                {
                    Reference = p.PaymentReference,
                    PaymentType = p.Type.ToString(),
                    Amount = p.Amount,
                    TransactionType = p.TransactionType.ToString(),
                })],
            Property = policy.Property is null ? null : new PropertyDto
            {
                AddressLine1 = policy.Property.AddressLine1 ?? string.Empty,
                AddressLine2 = policy.Property.AddressLine2 ?? string.Empty,
                AddressLine3 = policy.Property.AddressLine3 ?? string.Empty,
                Postcode = policy.Property.Postcode ?? string.Empty
            },
            CancellationDate = policy.CancellationDate,
        };
    }
}
