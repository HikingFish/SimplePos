using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using Xunit;

namespace Tests.Domain.Outlets;

public class OutletProductAvailabilityTests
{
    [Fact]
    public void Create_ShouldSucceed_WithDefaultAvailableState()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        Result<OutletProductAvailability> result = OutletProductAvailability.Create(outletId, productId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var availability = result.Data;

        Assert.Equal(outletId, availability.OutletId);
        Assert.Equal(productId, availability.ProductId);
        Assert.True(availability.IsAvailable);
        Assert.Null(availability.MarkedUnavailableAt);
        Assert.Null(availability.MarkedByUserId);
    }

    [Fact]
    public void Create_ShouldSucceed_WhenExplicitlyUnavailable()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        Result<OutletProductAvailability> result = OutletProductAvailability.Create(
            outletId, productId, isAvailable: false, markedByUserId: userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var availability = result.Data;

        Assert.Equal(outletId, availability.OutletId);
        Assert.Equal(productId, availability.ProductId);
        Assert.False(availability.IsAvailable);
        Assert.NotNull(availability.MarkedUnavailableAt);
        Assert.Equal(userId, availability.MarkedByUserId);
    }

    [Fact]
    public void Create_ShouldFail_WhenOutletIdIsEmpty()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        Result<OutletProductAvailability> result = OutletProductAvailability.Create(Guid.Empty, productId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OutletProductAvailabilityError.OutletIdEmpty, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenProductIdIsEmpty()
    {
        // Arrange
        var outletId = Guid.NewGuid();

        // Act
        Result<OutletProductAvailability> result = OutletProductAvailability.Create(outletId, Guid.Empty);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OutletProductAvailabilityError.ProductIdEmpty, result.Error);
    }

    [Theory]
    [InlineData(null)]
    public void Create_ShouldFail_WhenUnavailableWithoutUserId(Guid? userId)
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        Result<OutletProductAvailability> result = OutletProductAvailability.Create(
            outletId, productId, isAvailable: false, markedByUserId: userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OutletProductAvailabilityError.UserIdEmpty, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenUnavailableWithEmptyGuidUserId()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        Result<OutletProductAvailability> result = OutletProductAvailability.Create(
            outletId, productId, isAvailable: false, markedByUserId: Guid.Empty);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OutletProductAvailabilityError.UserIdEmpty, result.Error);
    }

    [Fact]
    public void MarkUnavailable_ShouldSucceed_WhenProductIsAvailable()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var availability = OutletProductAvailability.Create(outletId, productId).Data!;

        // Act
        Result result = availability.MarkUnavailable(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(availability.IsAvailable);
        Assert.NotNull(availability.MarkedUnavailableAt);
        Assert.Equal(userId, availability.MarkedByUserId);
    }

    [Fact]
    public void MarkUnavailable_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var availability = OutletProductAvailability.Create(outletId, productId).Data!;

        // Act
        Result result = availability.MarkUnavailable(Guid.Empty);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(OutletProductAvailabilityError.UserIdEmpty, result.Error);
        Assert.True(availability.IsAvailable);
    }

    [Fact]
    public void MarkUnavailable_ShouldFail_WhenAlreadyUnavailable()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var availability = OutletProductAvailability.Create(
            outletId, productId, isAvailable: false, markedByUserId: userId1).Data!;

        // Act
        Result result = availability.MarkUnavailable(userId2);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(OutletProductAvailabilityError.AlreadyUnavailable, result.Error);
    }

    [Fact]
    public void MarkAvailable_ShouldSucceed_WhenProductIsUnavailable()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var availability = OutletProductAvailability.Create(
            outletId, productId, isAvailable: false, markedByUserId: userId).Data!;

        // Act
        Result result = availability.MarkAvailable();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(availability.IsAvailable);
        Assert.Null(availability.MarkedUnavailableAt);
        Assert.Null(availability.MarkedByUserId);
    }

    [Fact]
    public void MarkAvailable_ShouldFail_WhenAlreadyAvailable()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var availability = OutletProductAvailability.Create(outletId, productId).Data!;

        // Act
        Result result = availability.MarkAvailable();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(OutletProductAvailabilityError.AlreadyAvailable, result.Error);
    }
}
