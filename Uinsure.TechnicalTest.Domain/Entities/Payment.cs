using Uinsure.TechnicalTest.Domain.Enums;
using Uninsure.TechnicalTest.Common.SharedKernal;

namespace Uinsure.TechnicalTest.Domain.Entities;

public class PaymentReference : Entity<string>
{
    public PaymentType Type { get; private set; }
    public decimal Amount { get; private set; }

    public PaymentReference() { }

    public PaymentReference(string paymentReference, PaymentType type, decimal amount) : base()
    {
        Id = paymentReference;
        Type = type;
        Amount = amount;
    }
}
