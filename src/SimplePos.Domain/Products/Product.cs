using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Products;
public class Product : ISoftDeletable
{
    public Guid ProductId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string SKU { get; private set; } 
    public string ProductName { get; private set; } 
    public decimal CostPrice { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }
    public bool SoftDeleted { get; private set; }
    public DateTime? DateTimeSoftDeleted { get; private set; }
    private readonly List<ProductTax> _productTaxes = new List<ProductTax>();
    public IReadOnlyCollection<ProductTax> ProductTaxes => _productTaxes.AsReadOnly();

    private Product() { }

    private Product(Guid productId, Guid companyId, Guid categoryId, string sku, string productName, decimal costPrice, decimal basePrice)
    {            
        ProductId = productId;
        CompanyId = companyId;
        CategoryId = categoryId;
        SKU = sku;
        ProductName = productName;
        CostPrice = costPrice;
        BasePrice = basePrice;
        IsActive = true;
        SoftDeleted = false;
        DateTimeSoftDeleted = null;
    }

    public static Result<Product> Create(Guid companyId, Guid categoryId, string sku, string productName, decimal costPrice, decimal basePrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            return Result<Product>.Failure(ProductError.ProductNameEmpty);
        }

        if (costPrice < 0)
        {
            return Result<Product>.Failure(ProductError.CostPriceNegative);
        }

        if (basePrice < 0)
        {
            return Result<Product>.Failure(ProductError.BasePriceNegative);
        }

        if (companyId == Guid.Empty)
        {
            return Result<Product>.Failure(ProductError.CompanyIdEmpty);
        }

        if (categoryId == Guid.Empty)
        {
            return Result<Product>.Failure(ProductError.CategoryIdEmpty);
        }

        return Result<Product>.Success(new Product(Guid.CreateVersion7(), companyId, categoryId, sku, productName, costPrice, basePrice));
    }

    public Result UpdateProductInfo(string newSKU, string newProductName, decimal newCostPrice, decimal newBasePrice)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (string.IsNullOrWhiteSpace(newProductName))
        {
            return Result.Failure(ProductError.ProductNameEmpty);
        }

        if (string.IsNullOrWhiteSpace(newSKU))
        {
            return Result.Failure(ProductError.SkuEmpty);
        }

        if (newCostPrice < 0)
        {
            return Result.Failure(ProductError.CostPriceNegative);
        }

        if (newBasePrice < 0)
        {
            return Result.Failure(ProductError.BasePriceNegative);
        }

        SKU = newSKU;
        ProductName = newProductName;
        CostPrice = newCostPrice;
        BasePrice = newBasePrice;
        return Result.Success();
    }

    public Result UpdateToActiveStatus()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (IsActive)
        {
            return Result.Failure(ProductError.AlreadyActive);
        }

        IsActive = true;
        return Result.Success();
    }

    public Result UpdateToNotActiveStatus()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (!IsActive)
        {
            return Result.Failure(ProductError.AlreadyInactive);
        }

        IsActive = false;
        return Result.Success();
    }

    public Result AddProductTax(ProductTax productTax)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (productTax == null)
        {
            return Result.Failure(ProductError.ProductTaxNull);
        }

        if (_productTaxes.Any(pt => pt.TaxId == productTax.TaxId))
        {
            return Result.Failure(ProductError.TaxAlreadyAssociated);
        }

        _productTaxes.Add(productTax);
        return Result.Success();
    }

    public Result RemoveProductTax(Guid taxId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        ProductTax productTax = _productTaxes.FirstOrDefault(pt => pt.TaxId == taxId);

        if (productTax == null)
        {
            return Result.Failure(ProductError.TaxNotAssociated);
        }

        _productTaxes.Remove(productTax);
        return Result.Success();
    }

    private Result EnsureNotSoftDeleted()
    {
        if (SoftDeleted)
        {
            return Result.Failure(ProductError.SoftDeleted);
        }
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
        IsActive = false;
        return Result.Success();
    }
}
