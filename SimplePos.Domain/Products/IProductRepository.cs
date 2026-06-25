namespace SimplePos.Domain.Products
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<List<Product>> GetProductsByOutletIdAsync(Guid outletId);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid productId);
        Task AddProductTaxAsync(ProductTax productTax);
        Task<List<ProductTax>> GetProductTaxesByProductIdAsync(Guid productId);
        Task DeleteProductTaxesByProductIdAsync(Guid productId);
    }
}