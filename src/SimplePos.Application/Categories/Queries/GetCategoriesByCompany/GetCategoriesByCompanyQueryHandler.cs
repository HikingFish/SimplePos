using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Categories.Common;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Categories.Queries.GetCategoriesByCompany;

public class GetCategoriesByCompanyQueryHandler : IQueryHandler<GetCategoriesByCompanyQuery, Result<List<CategoryResponse>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesByCompanyQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<CategoryResponse>>> HandleAsync(GetCategoriesByCompanyQuery query, CancellationToken cancellationToken)
    {
        List<Category> categories = await _categoryRepository.GetCategoryByCompanyIdAsync(query.CompanyId);

        List<CategoryResponse> responses = categories
            .Where(c => !c.SoftDeleted)
            .Select(c => new CategoryResponse(c.CategoryId, c.Name, c.IsActive))
            .ToList();

        return Result<List<CategoryResponse>>.Success(responses);
    }
}
