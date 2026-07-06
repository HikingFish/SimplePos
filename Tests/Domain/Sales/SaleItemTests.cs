using SimplePos.Domain.Sales;
using SimplePos.Domain.Common.ResultPattern;
using Xunit;

namespace Tests.Domain.Sales;

public class SaleItemTests
{
    [Fact]
    public void SaleItem_Should_Have_Correct_Properties()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var saleId = Guid.NewGuid();
        var quantity = 5m;
        var unitPrice = 10.0m;
        var unitDiscount = 1.0m; // no discount
        string? remark = "Sample remark";
        var taxRate = 0.05m; // 5% tax

        // Act
        Result<SaleItem> result = SaleItem.Create(productId, saleId, quantity, unitPrice, unitDiscount, remark, taxRate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var saleItem = result.Data;

        Assert.Equal(productId, saleItem.ProductId);
        Assert.Equal(saleId, saleItem.SaleId);
        Assert.Equal(quantity, saleItem.Quantity);
        Assert.Equal(unitPrice, saleItem.UnitPrice);
        Assert.Equal(unitDiscount, saleItem.UnitDiscount);
        Assert.Equal(remark, saleItem.Remark);
        Assert.Equal(taxRate, saleItem.TaxRate);
    }

    [Theory]
    [InlineData(5, 12, 0, "test", 0.06)]
    [InlineData(146, 0.7, 0, "test", 0.16)]
    [InlineData(1, 12, 0.15, "test", 0.06)]
    public void SaleItemCalculation_ShouldCalculateCorrect_WhenInputIsValidByBulk(decimal quantity, decimal unitPrice, decimal unitDiscount, string? remark, decimal taxRate)
    {
        //Arrange
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxRate);

        //Assert
        Assert.Equal(result.Data.GrossAmount, decimal.Round(quantity * unitPrice, 2));
    }

    [Fact]
    public void SaleItemCalculation_ShouldCalculateCorrect_WhenInputIsValid()
    {
        //Arrange
        decimal quantity = 5;
        decimal unitPrice = 12;
        decimal unitDiscount = 0.15M;
        string? remark = "test";
        decimal taxRate = 0.06M;
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxRate);

        //Assert
        Assert.Equal(10.2M, result.Data.DiscountedUnitPrice);
        Assert.Equal(60M, result.Data.GrossAmount);
        Assert.Equal(51M, result.Data.NetAmount);
        Assert.Equal(3.06M, result.Data.TaxAmount);
        Assert.Equal(54.06M, result.Data.TotalLineAmount);
    }
    [Fact]
    public void SaleItemCalculation_ShouldCalculateCorrect_WhenInputIsValidWithoutDiscount()
    {
        //Arrange
        decimal quantity = 5;
        decimal unitPrice = 12;
        decimal unitDiscount = 0M;
        string? remark = "test";
        decimal taxRate = 0.06M;
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxRate);

        //Assert
        Assert.Equal(12M, result.Data.DiscountedUnitPrice);
        Assert.Equal(60M, result.Data.GrossAmount);
        Assert.Equal(60M, result.Data.NetAmount);
        Assert.Equal(3.6M, result.Data.TaxAmount);
        Assert.Equal(63.6M, result.Data.TotalLineAmount);
    }
    [Fact]
    public void SaleItemCalculation_ShouldCalculateCorrect_WhenInputIsValidWithoutTax()
    {
        //Arrange
        decimal quantity = 5;
        decimal unitPrice = 12;
        decimal unitDiscount = 0.15M;
        string? remark = "test";
        decimal taxRate = 0M;
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxRate);

        //Assert
        Assert.Equal(10.2M, result.Data.DiscountedUnitPrice);
        Assert.Equal(60M, result.Data.GrossAmount);
        Assert.Equal(51M, result.Data.NetAmount);
        Assert.Equal(0M, result.Data.TaxAmount);
        Assert.Equal(51M, result.Data.TotalLineAmount);
    }
    [Fact]
    public void SaleItemCalculation_ShouldCalculateCorrect_WhenInputIsValidWithoutUnitPrice()
    {
        //Arrange
        decimal quantity = 5;
        decimal unitPrice = 0M;
        decimal unitDiscount = 0.15M;
        string? remark = "test";
        decimal taxRate = 0.06M;
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxRate);

        //Assert
        Assert.Equal(0M, result.Data.DiscountedUnitPrice);
        Assert.Equal(0M, result.Data.GrossAmount);
        Assert.Equal(0M, result.Data.NetAmount);
        Assert.Equal(0M, result.Data.TaxAmount);
        Assert.Equal(0M, result.Data.TotalLineAmount);
    }
}