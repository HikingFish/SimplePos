using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        Result<Category> categoryResult = Category.Create(command.CompanyId, command.Name);
        if (!categoryResult.IsSuccess)
        {
            return Result<Guid>.Failure(categoryResult.Error);
        }

        _categoryRepository.AddCategory(categoryResult.Data!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(categoryResult.Data!.CategoryId);
    }
}
