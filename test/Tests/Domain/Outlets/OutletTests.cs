using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using Xunit;

namespace Tests.Domain.Outlets;

public class OutletTests
{
    private Address CreateValidAddress() => 
        Address.Create("Jalan 8", "Ampang", "Selangor", "12345", "Malaysia").Data!;

    private PhoneNumber CreateValidPhone() => 
        PhoneNumber.Create("012345678").Data!;

    [Fact]
    public void Create_ShouldReturnOutlet_WhenInputIsValid()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        string name = "Main Branch";
        Address address = CreateValidAddress();
        PhoneNumber phone = CreateValidPhone();

        // Act
        Result<Outlet> result = Outlet.Create(companyId, name, address, phone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var outlet = result.Data;

        Assert.Equal(companyId, outlet.CompanyId);
        Assert.Equal(name, outlet.Name);
        Assert.Equal(address, outlet.OutletAddress);
        Assert.Equal(phone, outlet.PhoneNumber);
        Assert.True(outlet.IsActive);
        Assert.False(outlet.SoftDeleted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldFail_WhenNameIsEmpty(string? name)
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        Address address = CreateValidAddress();
        PhoneNumber phone = CreateValidPhone();

        // Act
        Result<Outlet> result = Outlet.Create(companyId, name!, address, phone);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OutletError.OutletNameEmpty, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenAddressIsNull()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        string name = "Main Branch";
        PhoneNumber phone = CreateValidPhone();

        // Act
        Result<Outlet> result = Outlet.Create(companyId, name, null!, phone);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OutletError.OutletAddressNull, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenPhoneNumberIsNull()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        string name = "Main Branch";
        Address address = CreateValidAddress();

        // Act
        Result<Outlet> result = Outlet.Create(companyId, name, address, null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(OutletError.OutletPhoneNumberNull, result.Error);
    }

    [Fact]
    public void UpdateOutletInfo_ShouldSucceed_WhenInputIsValid()
    {
        // Arrange
        var outlet = Outlet.Create(Guid.NewGuid(), "Main Branch", CreateValidAddress(), CreateValidPhone()).Data!;
        string newName = "Updated Branch";
        Address newAddress = Address.Create("Jalan 2", "Batu Caves", "Selangor", "12567", "Malaysia").Data!;
        PhoneNumber newPhone = PhoneNumber.Create("012987654").Data!;

        // Act
        Result result = outlet.UpdateOutletInfo(newName, newAddress, newPhone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, outlet.Name);
        Assert.Equal(newAddress, outlet.OutletAddress);
        Assert.Equal(newPhone, outlet.PhoneNumber);
    }

    [Fact]
    public void UpdateOutletInfo_ShouldFail_WhenPhoneNumberIsNull()
    {
        // Arrange
        var outlet = Outlet.Create(Guid.NewGuid(), "Main Branch", CreateValidAddress(), CreateValidPhone()).Data!;

        // Act
        Result result = outlet.UpdateOutletInfo("Updated Branch", CreateValidAddress(), null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(OutletError.OutletPhoneNumberNull, result.Error);
    }

    [Fact]
    public void UpdateOutletInfo_ShouldFail_WhenSoftDeleted()
    {
        // Arrange
        var outlet = Outlet.Create(Guid.NewGuid(), "Main Branch", CreateValidAddress(), CreateValidPhone()).Data!;
        outlet.SoftDelete();

        // Act
        Result result = outlet.UpdateOutletInfo("Updated Branch", CreateValidAddress(), CreateValidPhone());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(OutletError.SoftDeleted, result.Error);
    }
}
