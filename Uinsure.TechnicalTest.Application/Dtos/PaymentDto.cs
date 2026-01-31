
using Uinsure.TechnicalTest.Domain.Entities;
using Uinsure.TechnicalTest.Domain.Enums;

namespace Uinsure.TechnicalTest.Application.Dtos;

public class PaymentDto
{
    public string Reference { get; set; } = string.Empty;
    public PaymentType PaymentType { get; set; }
    public decimal Amount { get; set; }

    internal Payment ToDomain(Guid policyId)
    {
        return new Payment(Reference, PaymentType, Amount, policyId);
    }
}
