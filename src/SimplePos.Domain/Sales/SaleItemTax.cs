using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Sales;

public class SaleItemTax
{
    public Guid SaleItemTaxId { get; private set; }
    public Guid SaleItemId { get; private set; }
    public Guid TaxId { get; private set; }
    public string TaxName { get; private set; } = string.Empty;
    public decimal TaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }

    private SaleItemTax() { }

    private SaleItemTax(
        Guid saleItemTaxId,
        Guid saleItemId,
        Guid taxId,
        string taxName,
        decimal taxRate,
        decimal taxAmount)
    {
        SaleItemTaxId = saleItemTaxId;
        SaleItemId = saleItemId;
        TaxId = taxId;
        TaxName = taxName;
        TaxRate = taxRate;
        TaxAmount = taxAmount;
    }

    public static Result<SaleItemTax> Create(
        Guid saleItemId,
        Guid taxId,
        string taxName,
        decimal taxRate,
        decimal taxAmount = 0,
        Guid? saleItemTaxId = null)
    {
        if (saleItemId == Guid.Empty)
        {
            return Result<SaleItemTax>.Failure(SaleItemTaxError.SaleItemIdEmpty);
        }

        if (taxId == Guid.Empty)
        {
            return Result<SaleItemTax>.Failure(SaleItemTaxError.TaxIdEmpty);
        }

        if (string.IsNullOrWhiteSpace(taxName))
        {
            return Result<SaleItemTax>.Failure(SaleItemTaxError.TaxNameEmpty);
        }

        if (taxRate < 0)
        {
            return Result<SaleItemTax>.Failure(SaleItemTaxError.TaxRateNegative);
        }

        Guid finalId = saleItemTaxId ?? Guid.CreateVersion7();
        return Result<SaleItemTax>.Success(new SaleItemTax(finalId, saleItemId, taxId, taxName, taxRate, taxAmount));
    }

    public void UpdateTaxAmount(decimal netAmount)
    {
        TaxAmount = decimal.Round(netAmount * TaxRate, 2);
    }
}
