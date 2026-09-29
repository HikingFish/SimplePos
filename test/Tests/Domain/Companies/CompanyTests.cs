using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using Xunit;

namespace Tests.Domain.Companies;

public class CompanyTests
{
    private Address CreateValidAddress() => 
        Address.Create("Jalan 8", "Ampang", "Selangor", "12345", "Malaysia").Data!;

    private EmailAddress CreateValidEmail() => 
        EmailAddress.Create("John@gmail.com").Data!;

    private PhoneNumber CreateValidPhone() => 
        PhoneNumber.Create("012345678").Data!;

    [Fact]
    public void Create_ShouldReturnCompany_WhenInputIsValid()
    {
        // Arrange
        string name = "John's Food";
        Address address = CreateValidAddress();
        EmailAddress emailAddress = CreateValidEmail();
        PhoneNumber phoneNumber = CreateValidPhone();
        var before = DateTime.UtcNow;

        // Act
        Result<Company> result = Company.Create(name, address, phoneNumber, emailAddress);
        var after = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var company = result.Data;

        Assert.Equal(company.CompanyAddress, address);
        Assert.Equal(company.Email, emailAddress);
        Assert.Equal(company.Name, name);
        Assert.Equal(company.PhoneNumber, phoneNumber);
        Assert.InRange(company.DateTimeCreated, before, after);
        Assert.InRange(company.DateTimeLastOnline, before, after);
        Assert.False(company.SoftDeleted);
        Assert.Null(company.DateTimeSoftDeleted);
        Assert.True(company.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldFail_WhenNameEmpty(string? name)
    {
        // Arrange
        Address address = CreateValidAddress();
        EmailAddress emailAddress = CreateValidEmail();
        PhoneNumber phoneNumber = CreateValidPhone();

        // Act
        Result<Company> result = Company.Create(name!, address, phoneNumber, emailAddress);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(CompanyError.CompanyNameEmpty, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenCompanyAddressNull()
    {
        // Arrange
        EmailAddress emailAddress = CreateValidEmail();
        PhoneNumber phoneNumber = CreateValidPhone();
        string name = "John's Food";

        // Act
        Result<Company> result = Company.Create(name, null!, phoneNumber, emailAddress);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(CompanyError.CompanyAddressNull, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenCompanyEmailNull()
    {
        // Arrange
        Address address = CreateValidAddress();
        PhoneNumber phoneNumber = CreateValidPhone();
        string name = "John's Food";

        // Act
        Result<Company> result = Company.Create(name, address, phoneNumber, null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(CompanyError.CompanyEmailNull, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenCompanyPhoneNumberNull()
    {
        // Arrange
        Address address = CreateValidAddress();
        EmailAddress emailAddress = CreateValidEmail();
        string name = "John's Food";

        // Act
        Result<Company> result = Company.Create(name, address, null!, emailAddress);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(CompanyError.CompanyPhoneNumberNull, result.Error);
    }

    [Fact]
    public void UpdateCompanyInfo_ShouldReturnSuccess_WhenInputIsValid()
    {
        // Arrange
        string name = "John's Food";
        Address address = CreateValidAddress();
        EmailAddress emailAddress = CreateValidEmail();
        PhoneNumber phoneNumber = CreateValidPhone();
        Result<Company> companyResult = Company.Create(name, address, phoneNumber, emailAddress);
        Assert.True(companyResult.IsSuccess);
        var company = companyResult.Data!;

        string newName = "Jack's Food";
        Address newAddress = Address.Create("Jalan 2", "Batu Caves", "Selangor", "12567", "Malaysia").Data!;
        PhoneNumber newPhoneNumber = PhoneNumber.Create("012987654").Data!;

        // Act
        Result result = company.UpdateCompanyInfo(newName, newAddress, newPhoneNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newAddress, company.CompanyAddress);
        Assert.Equal(newName, company.Name);
        Assert.Equal(newPhoneNumber, company.PhoneNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdateCompanyInfo_ShouldFail_WhenNameEmpty(string? newName)
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        Address newAddress = Address.Create("Jalan 2", "Batu Caves", "Selangor", "12567", "Malaysia").Data!;

        // Act
        Result result = company.UpdateCompanyInfo(newName!, newAddress, CreateValidPhone());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CompanyError.CompanyNameEmpty, result.Error);
    }

    [Fact]
    public void UpdateCompanyInfo_ShouldFail_WhenAddressNull()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;

        // Act
        Result result = company.UpdateCompanyInfo("Jack's Food", null!, CreateValidPhone());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CompanyError.CompanyAddressNull, result.Error);
    }

    [Fact]
    public void UpdateCompanyInfo_ShouldFail_WhenPhoneNumberNull()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;

        // Act
        Result result = company.UpdateCompanyInfo("Jack's Food", CreateValidAddress(), null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CompanyError.CompanyPhoneNumberNull, result.Error);
    }

    [Fact]
    public void UpdateEmail_ShouldReturnSuccess_WhenEmailIsValid()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        var newEmail = EmailAddress.Create("jack@gmail.com").Data!;

        // Act
        Result result = company.UpdateEmail(newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, company.Email);
    }

    [Fact]
    public void UpdateEmail_ShouldFail_WhenEmailNull()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;

        // Act
        Result result = company.UpdateEmail(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CompanyError.CompanyEmailNull, result.Error);
    }

    [Fact]
    public void UpdateToNotActiveStatus_ShouldReturnSuccess_WhenActive()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        Assert.True(company.IsActive);

        // Act
        Result result = company.UpdateToNotActiveStatus();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(company.IsActive);
    }

    [Fact]
    public void UpdateToNotActiveStatus_ShouldFail_WhenAlreadyInactive()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        company.UpdateToNotActiveStatus();
        Assert.False(company.IsActive);

        // Act
        Result result = company.UpdateToNotActiveStatus();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CompanyError.AlreadyInactive, result.Error);
    }

    [Fact]
    public void UpdateToActiveStatus_ShouldReturnSuccess_WhenInactive()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        company.UpdateToNotActiveStatus();
        Assert.False(company.IsActive);

        // Act
        Result result = company.UpdateToActiveStatus();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(company.IsActive);
    }

    [Fact]
    public void UpdateToActiveStatus_ShouldFail_WhenAlreadyActive()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        Assert.True(company.IsActive);

        // Act
        Result result = company.UpdateToActiveStatus();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CompanyError.AlreadyActive, result.Error);
    }

    [Fact]
    public void UpdateLastOnline_ShouldReturnSuccess_AndUpdateTimestamp()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        var beforeUpdate = DateTime.UtcNow;

        // Act
        Result result = company.UpdateLastOnline();
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.InRange(company.DateTimeLastOnline, beforeUpdate, afterUpdate);
    }

    [Fact]
    public void SoftDelete_ShouldReturnSuccess_AndSetFlags()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        var beforeDelete = DateTime.UtcNow;

        // Act
        Result result = company.SoftDelete();
        var afterDelete = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(company.SoftDeleted);
        Assert.NotNull(company.DateTimeSoftDeleted);
        Assert.InRange(company.DateTimeSoftDeleted.Value, beforeDelete, afterDelete);
        Assert.False(company.IsActive);
    }

    [Fact]
    public void Operations_ShouldFail_WhenCompanyIsSoftDeleted()
    {
        // Arrange
        var company = Company.Create("John's Food", CreateValidAddress(), CreateValidPhone(), CreateValidEmail()).Data!;
        company.SoftDelete();

        // Act & Assert
        Result updateInfoResult = company.UpdateCompanyInfo("New Name", CreateValidAddress(), CreateValidPhone());
        Assert.False(updateInfoResult.IsSuccess);
        Assert.Equal(CompanyError.SoftDeleted, updateInfoResult.Error);

        Result updateEmailResult = company.UpdateEmail(CreateValidEmail());
        Assert.False(updateEmailResult.IsSuccess);
        Assert.Equal(CompanyError.SoftDeleted, updateEmailResult.Error);

        Result updateToActiveResult = company.UpdateToActiveStatus();
        Assert.False(updateToActiveResult.IsSuccess);
        Assert.Equal(CompanyError.SoftDeleted, updateToActiveResult.Error);

        Result updateToInactiveResult = company.UpdateToNotActiveStatus();
        Assert.False(updateToInactiveResult.IsSuccess);
        Assert.Equal(CompanyError.SoftDeleted, updateToInactiveResult.Error);

        Result updateLastOnlineResult = company.UpdateLastOnline();
        Assert.False(updateLastOnlineResult.IsSuccess);
        Assert.Equal(CompanyError.SoftDeleted, updateLastOnlineResult.Error);

        Result softDeleteResult = company.SoftDelete();
        Assert.False(softDeleteResult.IsSuccess);
        Assert.Equal(CompanyError.SoftDeleted, softDeleteResult.Error);
    }
}