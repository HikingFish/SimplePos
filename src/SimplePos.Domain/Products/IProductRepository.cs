namespace SimplePos.Domain.Products;

public interface IProductRepository
{
    void AddProduct(Product product);
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<Product?> GetProductBySkuAsync(string sku, Guid companyId);
    Task<List<Product>> GetProductsByCompanyIdAsync(Guid companyId);
    Task<List<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
    Task<bool> ExistsBySkuAsync(string sku, Guid companyId);
    Task<(List<Product> Products, int TotalCount)> GetPagedProductsByCompanyIdAsync(Guid companyId, int page, int pageSize, string? sortBy = null, bool isDescending = false);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
}
