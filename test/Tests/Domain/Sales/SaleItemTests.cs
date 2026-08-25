using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using SimplePos.Domain.Common.ResultPattern;
using Xunit;

namespace Tests.Domain.Sales;

public class SaleItemTests
{
    private static readonly Guid CompanyId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

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
        var taxes = new List<Tax> { Tax.Create(CompanyId, "VAT", 0.05m).Data! };

        // Act
        Result<SaleItem> result = SaleItem.Create(productId, saleId, UserId, quantity, unitPrice, unitDiscount, remark, taxes);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var saleItem = result.Data;

        Assert.Equal(productId, saleItem.ProductId);
        Assert.Equal(saleId, saleItem.SaleId);
        Assert.Equal(UserId, saleItem.AddedByUserId);
        Assert.False(saleItem.Void);
        Assert.Null(saleItem.VoidedByUserId);
        Assert.Null(saleItem.DateTimeVoided);
        Assert.Equal(quantity, saleItem.Quantity);
        Assert.Equal(unitPrice, saleItem.UnitPrice);
        Assert.Equal(unitDiscount, saleItem.UnitDiscount);
        Assert.Equal(remark, saleItem.Remark);
        Assert.Equal(0.05m, saleItem.TaxRate);
        Assert.Single(saleItem.SaleItemTaxes);
        Assert.Equal("VAT", saleItem.SaleItemTaxes.First().TaxName);
    }

    [Theory]
    [InlineData(5, 12, 0, "test", 0.06)]
    [InlineData(146, 0.7, 0, "test", 0.16)]
    [InlineData(1, 12, 0.15, "test", 0.06)]
    public void SaleItemCalculation_ShouldCalculateCorrect_WhenInputIsValidByBulk(decimal quantity, decimal unitPrice, decimal unitDiscount, string? remark, decimal taxRate)
    {
        //Arrange
        var taxes = new List<Tax> { Tax.Create(CompanyId, "Tax", taxRate).Data! };
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), UserId, quantity, unitPrice, unitDiscount, remark, taxes);

        //Assert
        Assert.NotNull(result.Data);
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
        var taxes = new List<Tax> { Tax.Create(CompanyId, "VAT", 0.06M).Data! };
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), UserId, quantity, unitPrice, unitDiscount, remark, taxes);

        //Assert
        Assert.NotNull(result.Data);
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
        var taxes = new List<Tax> { Tax.Create(CompanyId, "VAT", 0.06M).Data! };
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), UserId, quantity, unitPrice, unitDiscount, remark, taxes);

        //Assert
        Assert.NotNull(result.Data);
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
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), UserId, quantity, unitPrice, unitDiscount, remark, taxes: null);

        //Assert
        Assert.NotNull(result.Data);
        Assert.Equal(10.2M, result.Data.DiscountedUnitPrice);
        Assert.Equal(60M, result.Data.GrossAmount);
        Assert.Equal(51M, result.Data.NetAmount);
        Assert.Equal(0M, result.Data.TaxAmount);
        Assert.Equal(51M, result.Data.TotalLineAmount);
        Assert.Empty(result.Data.SaleItemTaxes);
    }

    [Fact]
    public void SaleItemCalculation_ShouldCalculateCorrect_WhenInputIsValidWithoutUnitPrice()
    {
        //Arrange
        decimal quantity = 5;
        decimal unitPrice = 0M;
        decimal unitDiscount = 0.15M;
        string? remark = "test";
        var taxes = new List<Tax> { Tax.Create(CompanyId, "VAT", 0.06M).Data! };
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), UserId, quantity, unitPrice, unitDiscount, remark, taxes);

        //Assert
        Assert.NotNull(result.Data);
        Assert.Equal(0M, result.Data.DiscountedUnitPrice);
        Assert.Equal(0M, result.Data.GrossAmount);
        Assert.Equal(0M, result.Data.NetAmount);
        Assert.Equal(0M, result.Data.TaxAmount);
        Assert.Equal(0M, result.Data.TotalLineAmount);
    }

    [Fact]
    public void SaleItemCalculation_ShouldCalculateMultipleTaxes_Correctly()
    {
        //Arrange
        decimal quantity = 2;
        decimal unitPrice = 100M;
        decimal unitDiscount = 0M;
        var tax1 = Tax.Create(CompanyId, "GST", 0.10M).Data!;     // 10%
        var tax2 = Tax.Create(CompanyId, "Service", 0.05M).Data!; // 5%
        var taxes = new List<Tax> { tax1, tax2 };

        //Act
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), UserId, quantity, unitPrice, unitDiscount, "Multiple taxes", taxes);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var saleItem = result.Data;

        Assert.Equal(200M, saleItem.NetAmount);
        Assert.Equal(0.15M, saleItem.TaxRate); // 10% + 5%
        Assert.Equal(30M, saleItem.TaxAmount); // 20 + 10 = 30
        Assert.Equal(230M, saleItem.TotalLineAmount);

        Assert.Equal(2, saleItem.SaleItemTaxes.Count);
        var gstTax = saleItem.SaleItemTaxes.First(t => t.TaxName == "GST");
        var serviceTax = saleItem.SaleItemTaxes.First(t => t.TaxName == "Service");
        Assert.Equal(20M, gstTax.TaxAmount);
        Assert.Equal(10M, serviceTax.TaxAmount);
    }

    [Fact]
    public void VoidSaleItem_ShouldSetVoidProperties()
    {
        // Arrange
        var saleItem = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), UserId, 1, 50m, 0m, "Test item").Data!;
        var voidingUser = Guid.NewGuid();

        // Act
        var result = saleItem.VoidSaleItem(voidingUser);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(saleItem.Void);
        Assert.Equal(voidingUser, saleItem.VoidedByUserId);
        Assert.NotNull(saleItem.DateTimeVoided);
    }

    [Fact]
    public void UpdateQuantity_ShouldRecalculateTotals_WhenQuantityIsValid()
    {
        // Arrange
        var taxes = new List<Tax> { Tax.Create(CompanyId, "VAT", 0.10M).Data! };
        var saleItem = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), UserId, 2, 50m, 0.10m, "Test item", taxes).Data!;
        Assert.Equal(2, saleItem.Quantity);
        Assert.Equal(90m, saleItem.NetAmount); // 2 * 45 = 90
        Assert.Equal(9m, saleItem.TaxAmount);   // 10% of 90 = 9
        Assert.Equal(99m, saleItem.TotalLineAmount);

        // Act
        var result = saleItem.UpdateQuantity(5);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, saleItem.Quantity);
        Assert.Equal(250m, saleItem.GrossAmount); // 5 * 50 = 250
        Assert.Equal(225m, saleItem.NetAmount);   // 5 * 45 = 225
        Assert.Equal(22.5m, saleItem.TaxAmount);  // 10% of 225 = 22.5
        Assert.Equal(247.5m, saleItem.TotalLineAmount);
        Assert.Equal(22.5m, saleItem.SaleItemTaxes.First().TaxAmount);
    }

    [Fact]
    public void UpdateQuantity_ShouldFail_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var saleItem = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), UserId, 2, 50m, 0m, "Test item").Data!;

        // Act
        var zeroResult = saleItem.UpdateQuantity(0);
        var negativeResult = saleItem.UpdateQuantity(-1);

        // Assert
        Assert.True(zeroResult.IsFailure);
        Assert.Equal(SaleItemError.QuantityZero, zeroResult.Error);
        Assert.True(negativeResult.IsFailure);
        Assert.Equal(SaleItemError.QuantityZero, negativeResult.Error);
    }

    [Fact]
    public void UpdateQuantity_ShouldFail_WhenSaleItemIsVoid()
    {
        // Arrange
        var saleItem = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), UserId, 2, 50m, 0m, "Test item").Data!;
        saleItem.VoidSaleItem(UserId);

        // Act
        var result = saleItem.UpdateQuantity(5);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleItemError.Void, result.Error);
    }
}