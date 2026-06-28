using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Category
{
    public static class CategoryError
    {
        public static readonly Error CategoryNameEmpty = new Error(
            "Category.CategoryNameEmpty", "Category name cannot be empty"
        );
        public static readonly Error CompanyIdEmpty = new Error(
            "Category.CompanyIdEmpty", "Company Id cannot be empty"
        );
        public static readonly Error SoftDeleted = new Error(
            "Category.SoftDeleted","Operation cannot be performed on a soft deleted category"
        );
    }
}