namespace SimplePos.Domain.Categories;

public interface ICategoryRepository
{
    Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    Task<List<Category>> GetCategoryByCompanyIdAsync(Guid companyId);
    void AddCategory(Category category);
    void UpdateCategory(Category category);
    void DeleteCategory(Category category);
}