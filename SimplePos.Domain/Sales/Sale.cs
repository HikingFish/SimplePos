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

    public Result RemoveSaleItem(SaleItem saleItem)
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

        if (!_saleItems.Contains(saleItem))
        {
            return Result.Failure(SaleError.SaleItemNotFound);
        }

        _saleItems.Remove(saleItem);
        CalculateTotals();
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

    private Result EnsureNotSoftDeleted()
    {
        if (SoftDeleted)
        {
            return Result.Failure(SaleError.SoftDeleted);
        }
        return Result.Success();
    }
}
