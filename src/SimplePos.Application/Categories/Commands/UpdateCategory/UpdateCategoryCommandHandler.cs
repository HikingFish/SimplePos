using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetCategoryByIdAsync(command.CategoryId);
        if (category == null || category.CompanyId != command.CompanyId || category.SoftDeleted)
        {
            return Result.Failure(CategoryError.NotExist);
        }

        Result updateResult = category.UpdateCategoryInfo(command.Name);
        if (!updateResult.IsSuccess)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
