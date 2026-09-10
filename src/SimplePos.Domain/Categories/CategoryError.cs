using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Categories;
public static class CategoryError
{
    public static readonly Error CategoryNameEmpty = new Error(
        "Category.CategoryNameEmpty", "Category name cannot be empty", ErrorType.Validation
    );
    public static readonly Error CompanyIdEmpty = new Error(
        "Category.CompanyIdEmpty", "Company Id cannot be empty", ErrorType.Validation
    );
    public static readonly Error SoftDeleted = new Error(
        "Category.SoftDeleted", "Operation cannot be performed on a soft deleted category", ErrorType.Conflict
    );
    public static readonly Error AlreadyActive = new Error(
        "Category.AlreadyActive", "Category is already active", ErrorType.Conflict
    );
    public static readonly Error AlreadyInactive = new Error(
        "Category.AlreadyInactive", "Category is already inactive", ErrorType.Conflict
    );
    public static readonly Error NotExist = new Error(
        "Category.NotExist", "Category does not exist", ErrorType.NotFound
    );
}