using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;
public class Sale
{
    public Guid SaleId { get; private set; }
    public Guid OutletId { get; private set; }
    public string InvoiceNumber { get; private set; } 
    // SubTotal is total without tax and discount
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    //Net Amount is total without tax but with discount
    public decimal NetAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime DateTimeCreated { get; private set; }
    public bool SoftDeleted { get; private set; }
    public decimal TotalPaid { get; private set; }
    public decimal TotalChange { get; private set; }
    public decimal TotalOutstanding { get; private set; }
    public decimal TotalDiscount { get; private set; }
    private readonly List<SaleItem> _saleItems  = new List<SaleItem>();
    public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();
    private readonly List<SalePayment> _salePayments = new List<SalePayment>();
    public IReadOnlyCollection<SalePayment> SalePayments => _salePayments.AsReadOnly();

    private Sale() { }

    private Sale(Guid saleId, Guid outletId, string invoiceNumber)
    {
        SaleId = saleId;
        OutletId = outletId;
        DateTimeCreated = DateTime.UtcNow;
        SoftDeleted = false;
        InvoiceNumber = invoiceNumber;
        SubTotal = 0;
        TaxAmount = 0;
        NetAmount = 0;
        TotalAmount = 0;
        TotalPaid = 0;
        TotalChange = 0;
        TotalOutstanding = 0;
    }

    public static Result<Sale> Create(Guid outletId, string invoiceNumber)
    {
        if (outletId == Guid.Empty)
        {
            return Result<Sale>.Failure(SaleError.OutletIdEmpty);
        }

        return Result<Sale>.Success(new Sale(Guid.CreateVersion7(), outletId, invoiceNumber));
    }

    public Result AddSaleItem(SaleItem saleItem)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
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

        if (payment == null)
        {
            return Result.Failure(SaleError.SalePaymentNull);
        }

        _salePayments.Add(payment);
        return Result.Success();
    }

    public Result RemoveSaleItem(Guid saleItemId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (saleItemId == Guid.Empty)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        SaleItem? saleItem = _saleItems.FirstOrDefault(s => s.SaleItemId == saleItemId);

        if (!_saleItems.Contains(saleItem))
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

        if (salePaymentId == Guid.Empty)
        {
            return Result.Failure(SaleError.SalePaymentNull);
        }

        SalePayment? salePayment = _salePayments.FirstOrDefault(s => s.SalePaymentId == salePaymentId);

        if (!_salePayments.Contains(salePayment))
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        _salePayments.Remove(salePayment);
        return Result.Success();
    }

    public Result UpdateSaleItem(Guid saleItemId, decimal quantity, decimal unitPrice, decimal unitDiscount, string? remark, decimal taxRate)
    {
        var statusResult = EnsureNotSoftDeleted();

        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (saleItemId == Guid.Empty)
        {
            return Result.Failure(SaleError.SaleItemNull);
        }

        if (quantity <= 0)
        {
            return Result.Failure(SaleItemError.QuantityZero);
        }

        //class would only create a pointer to it where as struct would create a copy of it.
        // So we can use the pointer to update the original object in the list.
        SaleItem? existingSaleItem = _saleItems.FirstOrDefault(s => s.SaleItemId == saleItemId);

        if (existingSaleItem == null)
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        Result updateResult = existingSaleItem.UpdateSaleItem(quantity, unitPrice, unitDiscount, remark, taxRate);
        if (!updateResult.IsSuccess)
        {
            return updateResult;
        }

        CalculateTotals();
        return Result.Success();
    }

    public Result SoftDelete()
    {
        if (SoftDeleted)
        {
            return Result.Failure(SaleError.SoftDeleted);
        }

        SoftDeleted = true;
        return Result.Success();
    }

    public Result CalculateTotals()
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

        foreach (var saleItem in _saleItems)
        {
            subTotal += saleItem.GrossAmount;
            taxAmount += saleItem.TaxAmount;
            netAmount += saleItem.NetAmount;
            totalAmount += saleItem.TotalLineAmount;
        }

        SubTotal = subTotal;
        TaxAmount = taxAmount;
        TotalAmount = totalAmount;
        NetAmount = netAmount;

        return Result.Success();
    }

    public Result CalculatePaymentTotals()
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

        totalChange = totalPaid > TotalAmount ? totalPaid - TotalAmount : 0;

        TotalPaid = totalPaid;
        TotalChange = totalChange;
        TotalOutstanding = totalOutstanding;

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
}
