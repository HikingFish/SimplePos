using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;
public class SalePayment
{
    public Guid SalePaymentId { get; private set; }
    public Guid SaleId { get; private set; }
    public Guid PaymentMethodId { get; private set; }
    public decimal AmountPaid { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? ReferenceNumber { get; private set; }

    private SalePayment() { }

    private SalePayment(Guid salePaymentId, Guid saleId, Guid paymentMethodId, decimal amountPaid, string? referenceNumber)
    {
        SalePaymentId = salePaymentId;
        SaleId = saleId;
        PaymentMethodId = paymentMethodId;
        AmountPaid = amountPaid;
        PaymentDate = DateTime.UtcNow;
        ReferenceNumber = referenceNumber;
    }

    public static Result<SalePayment> Create(Guid saleId, Guid paymentMethodId, decimal amount, string? referenceNumber = null)
    {
        if (saleId == Guid.Empty)
        {
            return Result<SalePayment>.Failure(SalePaymentError.SaleIdEmpty);
        }

        if (paymentMethodId == Guid.Empty)
        {
            return Result<SalePayment>.Failure(SalePaymentError.PaymentMethodIdEmpty);
        }

        if (amount <= 0)
        {
            return Result<SalePayment>.Failure(SalePaymentError.AmountNegativeOrZero);
        }

        return Result<SalePayment>.Success(new SalePayment(Guid.CreateVersion7(), saleId, paymentMethodId, amount, referenceNumber));
    }
}
