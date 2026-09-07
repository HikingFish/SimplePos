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

    public async Task<(List<Product> Products, int TotalCount)> GetPagedProductsByCompanyIdAsync(Guid companyId, int page, int pageSize, string? sortBy = null, bool isDescending = false)
    {
        IQueryable<Product> query = _appDbContext.Products
            .Include(p => p.ProductTaxes)
            .Where(p => p.CompanyId == companyId);

        int totalCount = await query.CountAsync();

        query = (sortBy?.ToLower(), isDescending) switch
        {
            ("sku", false) => query.OrderBy(p => p.SKU),
            ("sku", true)  => query.OrderByDescending(p => p.SKU),
            ("price", false) => query.OrderBy(p => p.BasePrice),
            ("price", true)  => query.OrderByDescending(p => p.BasePrice),
            (_, true)  => query.OrderByDescending(p => p.ProductName),
            _          => query.OrderBy(p => p.ProductName)
        };

        var safePage = page < 1 ? 1 : page;
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        var items = await query
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync();
        return (items, totalCount);
    }
}