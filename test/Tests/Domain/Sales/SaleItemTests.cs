using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using SimplePos.Domain.Common.ResultPattern;
using Xunit;

namespace Tests.Domain.Sales;

public class SaleItemTests
{
    private static readonly Guid CompanyId = Guid.NewGuid();

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
        Result<SaleItem> result = SaleItem.Create(productId, saleId, quantity, unitPrice, unitDiscount, remark, taxes);

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
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxes);

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
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxes);

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
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxes);

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
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxes: null);

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
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, remark, taxes);

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
        Result<SaleItem> result = SaleItem.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), quantity, unitPrice, unitDiscount, "Multiple taxes", taxes);

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
}