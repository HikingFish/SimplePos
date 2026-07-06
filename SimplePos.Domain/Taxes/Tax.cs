using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Taxes;
public class Tax
{
    public Guid TaxId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string TaxName { get; private set; }
    public decimal TaxRate { get; private set; }
    public bool IsActive { get; private set; }

    private Tax() { }

    private Tax(Guid taxId, Guid companyId, string taxName, decimal taxRate)
    {
        TaxId = taxId;
        CompanyId = companyId;
        TaxName = taxName;
        TaxRate = taxRate;
        IsActive = true;
    }

    public static Result<Tax> Create(Guid companyId, string taxName, decimal taxRate)
    {
        if (string.IsNullOrWhiteSpace(taxName))
        {
            return Result<Tax>.Failure(TaxError.TaxNameEmpty);
        }

        if (taxRate < 0)
        {
            return Result<Tax>.Failure(TaxError.TaxRateNegative);
        }

        return Result<Tax>.Success(new Tax(Guid.CreateVersion7(), companyId, taxName, taxRate));
    }

    public Result UpdateTaxInfo(string newTaxName, decimal newTaxRate)
    {
        if (string.IsNullOrWhiteSpace(newTaxName))
        {
            return Result.Failure(TaxError.TaxNameEmpty);
        }

        if (newTaxRate < 0)
        {
            return Result.Failure(TaxError.TaxRateNegative);
        }

        TaxName = newTaxName;
        TaxRate = newTaxRate;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(TaxError.AlreadyDeactivated);
        }

        IsActive = false;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(TaxError.AlreadyActivated);
        }

        IsActive = true;
        return Result.Success();
    }
}
