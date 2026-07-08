namespace SimplePos.Domain.Products;
public interface IProductRepository
{
    Task AddProductAsync(Product product);
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<List<Product>> GetProductsByCompanyIdAsync(Guid companyId);
    Task<Product?> GetProductBySkuAsync(string sku, Guid companyId);
    Task<List<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
    Task<bool> ExistsBySkuAsync(string sku, Guid companyId);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid productId);
    Task AddProductTaxAsync(ProductTax productTax);
    Task<List<ProductTax>> GetProductTaxesByProductIdAsync(Guid productId);
    Task DeleteProductTaxesByProductIdAsync(Guid productId);
}
