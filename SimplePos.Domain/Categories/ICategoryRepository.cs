using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Categories;
public interface ICategoryRepository
{
    Task<Result<Category>> GetCategoryByIdAsync(Guid categoryId);
    Task<Result<List<Category>>> GetCategoryByCompanyIdAsync(Guid companyId);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid categoryId);
}