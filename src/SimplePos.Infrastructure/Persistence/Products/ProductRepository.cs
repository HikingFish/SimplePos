using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Products;

namespace SimplePos.Infrastructure.Persistence.Products;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _appDbContext;

    public ProductRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddProduct(Product product)
    {
        _appDbContext.Products.Add(product);
    }

    public void UpdateProduct(Product product)
    {
        _appDbContext.Products.Update(product);
    }

    public void DeleteProduct(Product product)
    {
        _appDbContext.Products.Remove(product);
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        return await _appDbContext.Products
            .Include(p => p.ProductTaxes)
            .FirstOrDefaultAsync(p => p.ProductId == productId);
    }

    public async Task<Product?> GetProductBySkuAsync(string sku, Guid companyId)
    {
        return await _appDbContext.Products
            .Include(p => p.ProductTaxes)
            .FirstOrDefaultAsync(p => p.SKU == sku && p.CompanyId == companyId);
    }

    public async Task<List<Product>> GetProductsByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.Products
            .Include(p => p.ProductTaxes)
            .Where(p => p.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
    {
        return await _appDbContext.Products
            .Include(p => p.ProductTaxes)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<bool> ExistsBySkuAsync(string sku, Guid companyId)
    {
        return await _appDbContext.Products
            .AnyAsync(p => p.SKU == sku && p.CompanyId == companyId);
    }
}