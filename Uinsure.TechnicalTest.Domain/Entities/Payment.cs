using Uinsure.TechnicalTest.Domain.Enums;
using Uninsure.TechnicalTest.Common.SharedKernal;

namespace Uinsure.TechnicalTest.Domain.Entities;

public class Payment : Entity<long>
{
    public string PaymentReference { get; private set; }
    public PaymentType Type { get; private set; }
    public decimal Amount { get; private set; }
    public Guid PolicyId { get; private set; }

    public Payment() { }

    public Payment(string paymentReference, PaymentType type, decimal amount, Guid policyId) : base()
    {
        PaymentReference = paymentReference;
        Type = type;
        Amount = amount;
        PolicyId = policyId;
    }
}
