namespace SimplePos.Domain.Categories;
public interface ICategoryRepository
{
    Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    Task<List<Category>> GetCategoryByCompanyIdAsync(Guid companyId);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid categoryId);
}