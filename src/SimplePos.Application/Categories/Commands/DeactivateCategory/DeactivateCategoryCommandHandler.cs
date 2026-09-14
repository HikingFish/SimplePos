using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Categories.Commands.DeactivateCategory;

public class DeactivateCategoryCommandHandler : ICommandHandler<DeactivateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeactivateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetCategoryByIdAsync(command.CategoryId);
        if (category == null || category.CompanyId != command.CompanyId)
        {
            return Result.Failure(CategoryError.NotExist);
        }

        Result result = category.UpdateToNotActiveStatus();
        if (!result.IsSuccess)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
