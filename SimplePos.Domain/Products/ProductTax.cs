using SimplePos.Domain.Common;

namespace SimplePos.Domain.Products
{
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

        internal static ProductTax Create(Guid productId, Guid taxId)
        {
            if (productId == Guid.Empty)
            {
                throw new DomainException("Product ID cannot be empty.");
            }

            if (taxId == Guid.Empty)
            {
                throw new DomainException("Tax ID cannot be empty.");
            }

            return new ProductTax(productId, taxId);
        }
    }
}