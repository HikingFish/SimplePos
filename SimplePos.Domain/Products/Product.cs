using SimplePos.Domain.Common;

namespace SimplePos.Domain.Products
{
    public class Product
    {
        public Guid ProductId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string SKU { get; private set; } 
        public string ProductName { get; private set; } 
        public decimal CostPrice { get; private set; }
        public decimal BasePrice { get; private set; }
        public bool IsActive { get; private set; }
        public bool SoftDeleted { get; private set; }
        private readonly List<ProductTax> _productTaxes = new List<ProductTax>();
        public IReadOnlyCollection<ProductTax> ProductTaxes => _productTaxes.AsReadOnly();

        private Product() { }

        private Product(Guid companyId, string sku, string productName, decimal costPrice, decimal basePrice)
        {            
            ProductId = Guid.CreateVersion7();
            CompanyId = companyId;
            SKU = sku;
            ProductName = productName;
            CostPrice = costPrice;
            BasePrice = basePrice;
            IsActive = true;
            SoftDeleted = false;
        }

        public static Product Create(Guid companyId, string sku, string productName, decimal costPrice, decimal basePrice)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new DomainException("Product name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new DomainException("SKU cannot be empty.");
            }

            if (costPrice < 0)
            {
                throw new DomainException("Cost price cannot be negative.");
            }

            if (basePrice < 0)
            {
                throw new DomainException("Base price cannot be negative.");
            }

            return new Product(companyId, sku, productName, costPrice, basePrice);
        }

        public void UpdateProductInfo(string newSKU, string newProductName, decimal newCostPrice, decimal newBasePrice)
        {
            EnsureNotSoftDeleted();

            if (string.IsNullOrWhiteSpace(newProductName))
            {
                throw new DomainException("Product name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(newSKU))
            {
                throw new DomainException("SKU cannot be empty.");
            }

            if (newCostPrice < 0)
            {
                throw new DomainException("Cost price cannot be negative.");
            }

            if (newBasePrice < 0)
            {
                throw new DomainException("Base price cannot be negative.");
            }

            SKU = newSKU;
            ProductName = newProductName;
            CostPrice = newCostPrice;
            BasePrice = newBasePrice;
        }

        public void UpdateToActiveStatus()
        {
            EnsureNotSoftDeleted();

            if (IsActive)
            {
                throw new DomainException("Product is already active.");
            }

            IsActive = true;
        }

        public void UpdateToNotActiveStatus()
        {
            EnsureNotSoftDeleted();

            if (!IsActive)
            {
                throw new DomainException("Product is already inactive.");
            }

            IsActive = false;
        }

        public void AddProductTax(ProductTax productTax)
        {
            EnsureNotSoftDeleted();

            if (productTax == null)
            {
                throw new DomainException("Product tax cannot be null.");
            }

            if (_productTaxes.Any(pt => pt.TaxId == productTax.TaxId))
            {
                throw new DomainException("This tax is already associated with the product.");
            }

            _productTaxes.Add(productTax);
        }

        public void RemoveProductTax(Guid taxId)
        {
            EnsureNotSoftDeleted();

            ProductTax productTax = _productTaxes.FirstOrDefault(pt => pt.TaxId == taxId);

            if (productTax == null)
            {
                throw new DomainException("This tax is not associated with the product.");
            }

            _productTaxes.Remove(productTax);
        }

        private void EnsureNotSoftDeleted()
        {
            if (SoftDeleted)
            {
                throw new DomainException("Operation cannot be performed on a soft-deleted product.");
            }
        }

        public void SoftDelete()
        {
            EnsureNotSoftDeleted();

            SoftDeleted = true;
            IsActive = false;
        }
    }
}