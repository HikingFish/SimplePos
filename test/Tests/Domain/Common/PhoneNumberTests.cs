using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using Xunit;

namespace Tests.Domain.Common;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("012345678")]
    [InlineData("+60123456789")]
    [InlineData("+1-555-123-4567")]
    [InlineData("+1 555 123 4567")]
    [InlineData("(012) 345-6789")]
    [InlineData("123.456.7890")]
    [InlineData("123")]
    public void Create_ShouldSucceed_WhenPhoneNumberIsValid(string input)
    {
        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(input, result.Data.Value);
    }

    [Fact]
    public void Create_ShouldTrimWhitespace()
    {
        // Arrange
        string raw = "  012345678  ";

        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(raw);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("012345678", result.Data.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFailWithEmpty_WhenPhoneNumberIsNullOrEmptyOrWhitespace(string? input)
    {
        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(PhoneNumberError.Empty, result.Error);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12")]
    [InlineData("123456789012345678901")] // 21 chars
    [InlineData("---")]
    [InlineData("++123456")]
    [InlineData("123+4567")]
    [InlineData("012-abc-4567")]
    public void Create_ShouldFailWithInvalidFormat_WhenPhoneNumberIsInvalid(string input)
    {
        // Act
        Result<PhoneNumber> result = PhoneNumber.Create(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(PhoneNumberError.InvalidFormat, result.Error);
    }

    [Fact]
    public void Equals_ShouldBeTrue_WhenValuesAreEqual()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("012345678").Data!;
        var phone2 = PhoneNumber.Create("012345678").Data!;

        // Assert
        Assert.Equal(phone1, phone2);
        Assert.True(phone1 == phone2);
        Assert.Equal(phone1.GetHashCode(), phone2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var phone = PhoneNumber.Create("012345678").Data!;

        // Act & Assert
        Assert.Equal("012345678", phone.ToString());
    }
}
