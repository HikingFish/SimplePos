namespace SimplePos.Domain.Products;

public interface IProductRepository
{
    void AddProduct(Product product);
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<List<Product>> GetProductsByCompanyIdAsync(Guid companyId);
    Task<Product?> GetProductBySkuAsync(string sku, Guid companyId);
    Task<List<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
    Task<bool> ExistsBySkuAsync(string sku, Guid companyId);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    void AddProductTax(ProductTax productTax);
    Task<List<ProductTax>> GetProductTaxesByProductIdAsync(Guid productId);
    void DeleteProductTax(ProductTax productTax);
    void DeleteProductTaxes(IEnumerable<ProductTax> productTaxes);
}
