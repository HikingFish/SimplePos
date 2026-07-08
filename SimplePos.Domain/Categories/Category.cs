using System.ComponentModel.Design;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;

namespace SimplePos.Domain.Categories;
public class Category
{
    public Guid CategoryId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public bool SoftDeleted { get; private set; }

    private Category() { }

    private Category(Guid categoryId, Guid companyId, string name)
    {
        CategoryId = categoryId;
        CompanyId = companyId;
        Name = name;
        IsActive = true;
        SoftDeleted = false;
    }

    public static Result<Category> Create(Guid companyId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Category>.Failure(CategoryError.CategoryNameEmpty);
        }

        if (companyId == Guid.Empty)
        {
            return Result<Category>.Failure(CategoryError.CompanyIdEmpty);
        }

        return Result<Category>.Success(new Category(Guid.CreateVersion7(), companyId, name)); 
    }
    public Result UpdateCategoryInfo(string newName)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure(CategoryError.CategoryNameEmpty);
        }

        Name = newName;
        
        return Result.Success();
    }

    public Result UpdateToActiveStatus()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }
        if (IsActive)
        {
            return Result.Failure(CategoryError.AlreadyActive);
        }
        IsActive = true;
        return Result.Success();
    }

    public Result UpdateToNotActiveStatus()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }
        if (!IsActive)
        {
            return Result.Failure(CategoryError.AlreadyInactive);
        }
        IsActive = false;
        return Result.Success();
    }

    public Result SoftDelete()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }
        SoftDeleted = true;
        IsActive = false;
        return Result.Success();
    }
    private Result EnsureNotSoftDeleted()
    {
        if (SoftDeleted)
        {
            return Result.Failure(CategoryError.SoftDeleted);
        }
        return Result.Success();
    }
}