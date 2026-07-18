using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Categories;
using Xunit;

namespace Tests.Domain.Categories;

public class CategoryTests
{
    [Fact]
    public void Create_ShouldReturnCategory_WhenInputIsValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        string name = "Electronics";

        // Act
        Result<Category> result = Category.Create(companyId, name);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var category = result.Data;

        Assert.Equal(companyId, category.CompanyId);
        Assert.Equal(name, category.Name);
        Assert.True(category.IsActive);
        Assert.False(category.SoftDeleted);
        Assert.Null(category.DateTimeSoftDeleted);
        Assert.NotEqual(Guid.Empty, category.CategoryId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldFail_WhenNameEmpty(string? name)
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        Result<Category> result = Category.Create(companyId, name!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(CategoryError.CategoryNameEmpty, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenCompanyIdIsEmpty()
    {
        // Arrange
        var companyId = Guid.Empty;
        string name = "Electronics";

        // Act
        Result<Category> result = Category.Create(companyId, name);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(CategoryError.CompanyIdEmpty, result.Error);
    }

    [Fact]
    public void UpdateCategoryInfo_ShouldReturnSuccess_WhenInputIsValid()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;
        string newName = "Gadgets";

        // Act
        Result result = category.UpdateCategoryInfo(newName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, category.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdateCategoryInfo_ShouldFail_WhenNameEmpty(string? newName)
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;

        // Act
        Result result = category.UpdateCategoryInfo(newName!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryError.CategoryNameEmpty, result.Error);
    }

    [Fact]
    public void UpdateToNotActiveStatus_ShouldReturnSuccess_WhenActive()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;
        Assert.True(category.IsActive);

        // Act
        Result result = category.UpdateToNotActiveStatus();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(category.IsActive);
    }

    [Fact]
    public void UpdateToNotActiveStatus_ShouldFail_WhenAlreadyInactive()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;
        category.UpdateToNotActiveStatus();
        Assert.False(category.IsActive);

        // Act
        Result result = category.UpdateToNotActiveStatus();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryError.AlreadyInactive, result.Error);
    }

    [Fact]
    public void UpdateToActiveStatus_ShouldReturnSuccess_WhenInactive()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;
        category.UpdateToNotActiveStatus();
        Assert.False(category.IsActive);

        // Act
        Result result = category.UpdateToActiveStatus();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(category.IsActive);
    }

    [Fact]
    public void UpdateToActiveStatus_ShouldFail_WhenAlreadyActive()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;
        Assert.True(category.IsActive);

        // Act
        Result result = category.UpdateToActiveStatus();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryError.AlreadyActive, result.Error);
    }

    [Fact]
    public void SoftDelete_ShouldReturnSuccess_AndSetFlags()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;
        var beforeDelete = DateTime.UtcNow;

        // Act
        Result result = category.SoftDelete();
        var afterDelete = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(category.SoftDeleted);
        Assert.NotNull(category.DateTimeSoftDeleted);
        Assert.InRange(category.DateTimeSoftDeleted.Value, beforeDelete, afterDelete);
        Assert.False(category.IsActive);
    }

    [Fact]
    public void Operations_ShouldFail_WhenCategoryIsSoftDeleted()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Electronics").Data!;
        category.SoftDelete();

        // Act & Assert
        Result updateInfoResult = category.UpdateCategoryInfo("New Name");
        Assert.False(updateInfoResult.IsSuccess);
        Assert.Equal(CategoryError.SoftDeleted, updateInfoResult.Error);

        Result updateToActiveResult = category.UpdateToActiveStatus();
        Assert.False(updateToActiveResult.IsSuccess);
        Assert.Equal(CategoryError.SoftDeleted, updateToActiveResult.Error);

        Result updateToInactiveResult = category.UpdateToNotActiveStatus();
        Assert.False(updateToInactiveResult.IsSuccess);
        Assert.Equal(CategoryError.SoftDeleted, updateToInactiveResult.Error);

        Result softDeleteResult = category.SoftDelete();
        Assert.False(softDeleteResult.IsSuccess);
        Assert.Equal(CategoryError.SoftDeleted, softDeleteResult.Error);
    }
}
