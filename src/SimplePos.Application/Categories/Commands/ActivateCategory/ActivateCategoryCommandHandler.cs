using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Categories.Commands.ActivateCategory;

public class ActivateCategoryCommandHandler : ICommandHandler<ActivateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ActivateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetCategoryByIdAsync(command.CategoryId);
        if (category == null || category.CompanyId != command.CompanyId)
        {
            return Result.Failure(CategoryError.NotExist);
        }

        Result result = category.UpdateToActiveStatus();
        if (!result.IsSuccess)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
