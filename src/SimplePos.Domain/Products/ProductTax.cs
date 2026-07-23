using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Products;
public class ProductTax
{
    public Guid ProductId { get; private set; }
    public Guid TaxId { get; private set; }
    private ProductTax() { }
    private ProductTax(Guid productId, Guid taxId)
    {
        ProductId = productId;
        TaxId = taxId;
    }

    internal static Result<ProductTax> Create(Guid productId, Guid taxId)
    {
        if (productId == Guid.Empty)
        {
            return Result<Guid>.Failure(ProductTaxError.ProductIdEmpty);
        }

        if (taxId == Guid.Empty)
        {
            return Result<Guid>.Failure(ProductTaxError.TaxIdEmpty);
        }

        return Result<ProductTax>.Success(new ProductTax(productId, taxId));
    }
}
