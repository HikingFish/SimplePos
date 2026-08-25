using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;

namespace SimplePos.Domain.Sales;
public class Sale : AggregateRoot, ISoftDeletable 
{
    public Guid SaleId { get; private set; }
    public Guid OutletId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string InvoiceNumber { get; private set; } 
    // SubTotal is total without tax and discount
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    //Net Amount is total without tax but with discount
    public decimal NetAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime DateTimeCreated { get; private set; }
    public bool SoftDeleted { get; private set; }
    public DateTime? DateTimeSoftDeleted { get; private set; }
    public bool Void { get; private set; }
    public Guid? VoidedByUserId { get; private set; }
    public DateTime? DateTimeVoided { get; private set; }
    public bool Closed { get; private set; }
    public Guid? ClosedByUserId { get; private set; }
    public DateTime? DateTimeClosed { get; private set; }
    public decimal TotalPaid { get; private set; }
    public decimal TotalChange { get; private set; }
    public decimal TotalOutstanding { get; private set; }
    public decimal TotalDiscount { get; private set; }
    private readonly List<SaleItem> _saleItems  = new List<SaleItem>();
    public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();
    private readonly List<SalePayment> _salePayments = new List<SalePayment>();
    public IReadOnlyCollection<SalePayment> SalePayments => _salePayments.AsReadOnly();

    private Sale() { }

    private Sale(Guid saleId, Guid outletId, Guid createdByUserId, string invoiceNumber, DateTime? dateTimeCreated = null)
    {
        SaleId = saleId;
        OutletId = outletId;
        CreatedByUserId = createdByUserId;
        DateTimeCreated = dateTimeCreated ?? DateTime.UtcNow;
        SoftDeleted = false;
        DateTimeSoftDeleted = null;
        InvoiceNumber = invoiceNumber;
        SubTotal = 0;
        TaxAmount = 0;
        NetAmount = 0;
        TotalAmount = 0;
        TotalPaid = 0;
        TotalChange = 0;
        TotalOutstanding = 0;
        TotalDiscount = 0;
        Void = false;
        VoidedByUserId = null;
        DateTimeVoided = null;
        Closed = false;
        ClosedByUserId = null;
        DateTimeClosed = null;
    }

    public static Result<Sale> Create(Guid outletId, Guid createdByUserId, string invoiceNumber, DateTime? dateTimeCreated = null, Guid? saleId = null)
    {
        if (outletId == Guid.Empty)
        {
            return Result<Sale>.Failure(SaleError.OutletIdEmpty);
        }

        if (createdByUserId == Guid.Empty)
        {
            return Result<Sale>.Failure(SaleError.CreatedByUserIdEmpty);
        }

        Guid finalId = saleId ?? Guid.CreateVersion7();
        return Result<Sale>.Success(new Sale(finalId, outletId, createdByUserId, invoiceNumber, dateTimeCreated));
    }

    public Result AddSaleItem(SaleItem saleItem)
    {
        var statusResult = EnsureNotSoftDeleted();

        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();

        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();

        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (saleItem == null)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        _saleItems.Add(saleItem);
        CalculateTotals();
        return Result.Success();
    }

    public Result AddPayment(SalePayment payment)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();

        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();

        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (payment == null)
        {
            return Result.Failure(SaleError.SalePaymentNull);
        }

        if (payment.AmountPaid <= 0)
        {
            return Result.Failure(SaleError.AmountNegativeOrZero);
        }

        if (TotalOutstanding <= 0)
        {
            return Result.Failure(SaleError.SaleFullyPaid);
        }

        _salePayments.Add(payment);
        CalculatePaymentTotals();
        return Result.Success();
    }

    public Result EditSaleItemQuantity(Guid saleItemId, decimal quantity)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();

        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();

        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (saleItemId == Guid.Empty)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        SaleItem? saleItem = _saleItems.FirstOrDefault(si => si.SaleItemId == saleItemId);

        if (saleItem == null)
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        var updateResult = saleItem.UpdateQuantity(quantity);
        if (!updateResult.IsSuccess)
        {
            return updateResult;
        }

        CalculateTotals();
        return Result.Success();
    }

    public Result RemoveSaleItem(Guid saleItemId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();

        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();

        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (saleItemId == Guid.Empty)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        SaleItem? saleItem = _saleItems.FirstOrDefault(s => s.SaleItemId == saleItemId);

        if (saleItem == null)
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        _saleItems.Remove(saleItem);
        CalculateTotals();
        return Result.Success();
    }

    public Result RemovePayment(Guid salePaymentId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();

        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();

        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (salePaymentId == Guid.Empty)
        {
            return Result.Failure(SaleError.SalePaymentNull);
        }

        SalePayment? salePayment = _salePayments.FirstOrDefault(s => s.SalePaymentId == salePaymentId);

        if (salePayment == null)
        {
            return Result.Failure(SaleError.SalePaymentNotFound);
        }

        _salePayments.Remove(salePayment);
        CalculatePaymentTotals();
        return Result.Success();
    }

    public Result UpdateSaleItem(Guid saleItemId, decimal quantity, decimal unitPrice, decimal unitDiscount, string? remark, IEnumerable<Tax>? taxes = null)
    {
        var statusResult = EnsureNotSoftDeleted();

        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();

        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();

        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (saleItemId == Guid.Empty)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        //class would only create a pointer to it where as struct would create a copy of it.
        // So we can use the pointer to update the original object in the list.
        SaleItem? existingSaleItem = _saleItems.FirstOrDefault(s => s.SaleItemId == saleItemId);

        if (existingSaleItem == null)
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        Result updateResult = existingSaleItem.UpdateSaleItem(quantity, unitPrice, unitDiscount, remark, taxes);
        if (!updateResult.IsSuccess)
        {
            return updateResult;
        }

        CalculateTotals();
        return Result.Success();
    }

    public Result SoftDelete()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        SoftDeleted = true;
        DateTimeSoftDeleted = DateTime.UtcNow;
        return Result.Success();
    }

    private Result CalculateTotals()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        decimal subTotal = 0;
        decimal taxAmount = 0;
        decimal totalAmount = 0;
        decimal netAmount = 0;
        decimal totalDiscount = 0;

        foreach (var saleItem in _saleItems.Where(si => !si.Void))
        {
            subTotal += saleItem.GrossAmount;
            taxAmount += saleItem.TaxAmount;
            netAmount += saleItem.NetAmount;
            totalAmount += saleItem.TotalLineAmount;
            totalDiscount += saleItem.TotalDiscount;
        }

        SubTotal = subTotal;
        TaxAmount = taxAmount;
        TotalAmount = totalAmount;
        NetAmount = netAmount;
        TotalDiscount = totalDiscount;

        CalculatePaymentTotals();

        return Result.Success();
    }

    private Result CalculatePaymentTotals()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        decimal totalPaid = 0;
        decimal totalChange = 0;
        decimal totalOutstanding = 0;

        foreach (var salePayment in _salePayments)
        {
            totalPaid += salePayment.AmountPaid;
        }

        totalOutstanding = TotalAmount - totalPaid;

        if(totalOutstanding < 0)
        {
            totalOutstanding = 0;
        }

        totalChange = totalPaid > TotalAmount ? totalPaid - TotalAmount : 0;

        TotalPaid = totalPaid;
        TotalChange = totalChange;
        TotalOutstanding = totalOutstanding;

        return Result.Success();
    }

    public Result VoidSale(Guid voidedByUserId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notClosed = EnsureNotClosed();
        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (voidedByUserId == Guid.Empty)
        {
            return Result.Failure(SaleError.VoidedByUserIdEmpty);
        }

        if (Void)
        {
            return Result.Failure(SaleError.Void);
        }

        Void = true;
        VoidedByUserId = voidedByUserId;
        DateTimeVoided = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UnvoidSale()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notClosed = EnsureNotClosed();
        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (!Void)
        {
            return Result.Failure(SaleError.NotVoid);
        }

        Void = false;
        VoidedByUserId = null;
        DateTimeVoided = null;
        return Result.Success();
    }

    public Result CloseSale(Guid closedByUserId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();
        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();
        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (closedByUserId == Guid.Empty)
        {
            return Result.Failure(SaleError.ClosedByUserIdEmpty);
        }

        Closed = true;
        ClosedByUserId = closedByUserId;
        DateTimeClosed = DateTime.UtcNow;
        return Result.Success();
    }

    public Result VoidSaleItem(Guid saleItemId, Guid voidedByUserId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();
        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();
        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (saleItemId == Guid.Empty)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        var saleItem = _saleItems.FirstOrDefault(s => s.SaleItemId == saleItemId);
        if (saleItem == null)
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        var voidResult = saleItem.VoidSaleItem(voidedByUserId);
        if (!voidResult.IsSuccess)
        {
            return voidResult;
        }

        CalculateTotals();
        return Result.Success();
    }

    public Result UnvoidSaleItem(Guid saleItemId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var notVoid = EnsureNotVoid();
        if (!notVoid.IsSuccess)
        {
            return notVoid;
        }

        var notClosed = EnsureNotClosed();
        if (!notClosed.IsSuccess)
        {
            return notClosed;
        }

        if (saleItemId == Guid.Empty)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        var saleItem = _saleItems.FirstOrDefault(s => s.SaleItemId == saleItemId);
        if (saleItem == null)
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        var unvoidResult = saleItem.UnvoidSaleItem();
        if (!unvoidResult.IsSuccess)
        {
            return unvoidResult;
        }

        CalculateTotals();
        return Result.Success();
    }

    private Result EnsureNotSoftDeleted()
    {
        if (SoftDeleted)
        {
            return Result.Failure(SaleError.SoftDeleted);
        }
        return Result.Success();
    }

    private Result EnsureNotVoid()
    {
        if (Void)
        {
            return Result.Failure(SaleError.Void);
        }
        return Result.Success();
    }

    private Result EnsureNotClosed()
    {
        if (Closed)
        {
            return Result.Failure(SaleError.Closed);
        }
        return Result.Success();
    }
}
