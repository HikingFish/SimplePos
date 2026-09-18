using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Categories.Common;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetCategoryByIdAsync(query.CategoryId);
        if (category == null || category.CompanyId != query.CompanyId || category.SoftDeleted)
        {
            return Result<CategoryResponse>.Failure(CategoryError.NotExist);
        }

        var response = new CategoryResponse(category.CategoryId, category.Name, category.IsActive);
        return Result<CategoryResponse>.Success(response);
    }
}
