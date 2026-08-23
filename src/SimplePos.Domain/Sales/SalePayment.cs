using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;
public class SalePayment
{
    public Guid SalePaymentId { get; private set; }
    public Guid SaleId { get; private set; }
    public Guid PaymentMethodId { get; private set; }
    public Guid ProcessedByUserId { get; private set; }
    public decimal AmountPaid { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? ReferenceNumber { get; private set; }

    private SalePayment() { }

    private SalePayment(
        Guid salePaymentId,
        Guid saleId,
        Guid paymentMethodId,
        Guid processedByUserId,
        decimal amountPaid,
        DateTime paymentDate,
        string? referenceNumber)
    {
        SalePaymentId = salePaymentId;
        SaleId = saleId;
        PaymentMethodId = paymentMethodId;
        ProcessedByUserId = processedByUserId;
        AmountPaid = amountPaid;
        PaymentDate = paymentDate;
        ReferenceNumber = referenceNumber;
    }

    public static Result<SalePayment> Create(
        Guid saleId,
        Guid paymentMethodId,
        Guid processedByUserId,
        decimal amount,
        string? referenceNumber = null,
        DateTime? paymentDate = null,
        Guid? salePaymentId = null)
    {
        if (saleId == Guid.Empty)
        {
            return Result<SalePayment>.Failure(SalePaymentError.SaleIdEmpty);
        }

        if (paymentMethodId == Guid.Empty)
        {
            return Result<SalePayment>.Failure(SalePaymentError.PaymentMethodIdEmpty);
        }

        if (processedByUserId == Guid.Empty)
        {
            return Result<SalePayment>.Failure(SalePaymentError.ProcessedByUserIdEmpty);
        }

        if (amount <= 0)
        {
            return Result<SalePayment>.Failure(SalePaymentError.AmountNegativeOrZero);
        }

        Guid finalId = salePaymentId ?? Guid.CreateVersion7();
        DateTime finalPaymentDate = paymentDate ?? DateTime.UtcNow;
        return Result<SalePayment>.Success(new SalePayment(finalId, saleId, paymentMethodId, processedByUserId, amount, finalPaymentDate, referenceNumber));
    }
}
