using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;

namespace SimplePos.Domain.Sales;
public class SaleItem
{
    public Guid SaleItemId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid SaleId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    //Gross amount total without discount and tax
    public decimal GrossAmount { get; private set; }
    //How much to discount from the unit price, e.g. 0.1 for 10% discount
    public decimal UnitDiscount { get; private set; }
    //Discounted unit price after applying the unit discount
    public decimal DiscountedUnitPrice { get; private set; }
    //Net Amount is total without tax but with discount
    public decimal NetAmount { get; private set; }
    public decimal TotalDiscount { get; private set; }
    public decimal TotalLineAmount { get; private set; }
    public string? Remark { get; private set; } 
    public decimal TaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }

    private readonly List<SaleItemTax> _saleItemTaxes = new List<SaleItemTax>();
    public IReadOnlyCollection<SaleItemTax> SaleItemTaxes => _saleItemTaxes.AsReadOnly();

    private SaleItem() { }

    private SaleItem(
        Guid saleItemId,
        Guid productId, 
        Guid saleId, 
        decimal quantity,
        decimal unitPrice,
        decimal unitDiscount,
        string? remark,
        IEnumerable<Tax>? taxes)
    {
        SaleItemId = saleItemId;
        ProductId = productId;
        SaleId = saleId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        UnitDiscount = unitDiscount;
        Remark = remark;

        if (taxes != null)
        {
            foreach (var tax in taxes)
            {
                var taxResult = SaleItemTax.Create(saleItemId, tax.TaxId, tax.TaxName, tax.TaxRate, 0);
                if (taxResult.IsSuccess && taxResult.Data != null)
                {
                    _saleItemTaxes.Add(taxResult.Data);
                }
            }
        }

        CalculateLineTotal();
    }

    public static Result<SaleItem> Create(
        Guid productId, 
        Guid saleId, 
        decimal quantity,
        decimal unitPrice,
        decimal unitDiscount,
        string? remark,
        IEnumerable<Tax>? taxes = null,
        Guid? saleItemId = null)
    {
        if (productId == Guid.Empty)
        {
            return Result<SaleItem>.Failure(SaleItemError.ProductIdEmpty);
        }

        if (saleId == Guid.Empty)
        {
            return Result<SaleItem>.Failure(SaleItemError.SaleIdEmpty);
        }

        if (unitDiscount < 0)
        {
            return Result<SaleItem>.Failure(SaleItemError.UnitDiscountNegative);
        }

        if (unitPrice < 0)
        {
            return Result<SaleItem>.Failure(SaleItemError.UnitPriceNegative);
        }

        if (taxes != null)
        {
            foreach (var tax in taxes)
            {
                if (tax.TaxRate < 0)
                {
                    return Result<SaleItem>.Failure(SaleItemError.TaxRateNegative);
                }
                if (tax.TaxId == Guid.Empty)
                {
                    return Result<SaleItem>.Failure(SaleItemTaxError.TaxIdEmpty);
                }
                if (string.IsNullOrWhiteSpace(tax.TaxName))
                {
                    return Result<SaleItem>.Failure(SaleItemTaxError.TaxNameEmpty);
                }
            }
        }

        Guid finalId = saleItemId ?? Guid.CreateVersion7();
        return Result<SaleItem>.Success(new SaleItem(finalId, productId, saleId, quantity, unitPrice, unitDiscount, remark, taxes));
    }

    private void CalculateLineTotal()
    {
        if (UnitDiscount > 0)
        {
            DiscountedUnitPrice = decimal.Round(UnitPrice - (UnitPrice * UnitDiscount), 2);
        }
        else
        {
            DiscountedUnitPrice = UnitPrice;
        }
        GrossAmount = decimal.Round(Quantity * UnitPrice, 2);
        NetAmount = decimal.Round(Quantity * DiscountedUnitPrice, 2);

        decimal totalTaxAmount = 0;
        decimal totalTaxRate = 0;

        foreach (var saleItemTax in _saleItemTaxes)
        {
            saleItemTax.UpdateTaxAmount(NetAmount);
            totalTaxAmount += saleItemTax.TaxAmount;
            totalTaxRate += saleItemTax.TaxRate;
        }

        TaxRate = totalTaxRate;
        TaxAmount = decimal.Round(totalTaxAmount, 2);

        TotalLineAmount = decimal.Round(NetAmount + TaxAmount, 2);
        TotalDiscount = decimal.Round(Quantity * (UnitPrice - DiscountedUnitPrice), 2);
    }

    public Result UpdateSaleItem(
        decimal quantity,
        decimal unitPrice,
        decimal unitDiscount,
        string? remark,
        IEnumerable<Tax>? taxes = null)
    {
        if (unitDiscount < 0)
        {
            return Result.Failure(SaleItemError.UnitDiscountNegative);
        }

        if (unitPrice < 0)
        {
            return Result.Failure(SaleItemError.UnitPriceNegative);
        }

        if (taxes != null)
        {
            foreach (var tax in taxes)
            {
                if (tax.TaxRate < 0)
                {
                    return Result.Failure(SaleItemError.TaxRateNegative);
                }
                if (tax.TaxId == Guid.Empty)
                {
                    return Result.Failure(SaleItemTaxError.TaxIdEmpty);
                }
                if (string.IsNullOrWhiteSpace(tax.TaxName))
                {
                    return Result.Failure(SaleItemTaxError.TaxNameEmpty);
                }
            }
        }

        Quantity = quantity;
        UnitPrice = unitPrice;
        UnitDiscount = unitDiscount;
        Remark = remark;

        _saleItemTaxes.Clear();
        if (taxes != null)
        {
            foreach (var tax in taxes)
            {
                var taxResult = SaleItemTax.Create(SaleItemId, tax.TaxId, tax.TaxName, tax.TaxRate, 0);
                if (taxResult.IsSuccess && taxResult.Data != null)
                {
                    _saleItemTaxes.Add(taxResult.Data);
                }
            }
        }

        CalculateLineTotal();

        return Result.Success();
    }
}
