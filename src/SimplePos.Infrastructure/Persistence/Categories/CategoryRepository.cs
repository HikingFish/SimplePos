using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Categories;

namespace SimplePos.Infrastructure.Persistence.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _appDbContext;

    public CategoryRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddCategory(Category category)
    {
        _appDbContext.Categories.Add(category);
    }

    public void DeleteCategory(Category category)
    {
        _appDbContext.Categories.Remove(category);
    }

    public async Task<List<Category>> GetCategoryByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.Categories
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
    {
        return await _appDbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
    }

    public void UpdateCategory(Category category)
    {
        _appDbContext.Categories.Update(category);
    }
}