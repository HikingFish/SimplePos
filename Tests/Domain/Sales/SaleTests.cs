using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;

namespace Tests.Domain.Sales;

public class SaleTests
{
    [Fact]
    public void Sale_Should_Have_Correct_Properties()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";

        // Act
        Result<Sale> result = Sale.Create(outletId, invoiceNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var sale = result.Data;

        Assert.Equal(outletId, sale.OutletId);
        Assert.Equal(invoiceNumber, sale.InvoiceNumber);
        Assert.False(sale.SoftDeleted);
        Assert.NotEqual(Guid.Empty, sale.SaleId);
    }
    [Fact]
    public void AddSaleItem_ShouldBeAdded_InputValidSaleItem()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        Result<Sale> result = Sale.Create(outletId, invoiceNumber);
        SaleItem saleItem = SaleItem.Create(Guid.NewGuid(), result.Data.SaleId, 5, 10.0m, 1.0m, "Sample remark", 0.05m).Data;

        // Act
        var resultAddingSaleItem = result.Data.AddSaleItem(saleItem);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var sale = result.Data;
        Assert.True(resultAddingSaleItem.IsSuccess);
        Assert.Equal(1, sale.SaleItems.Count);
        Assert.Equal(outletId, sale.OutletId);
        Assert.Equal(invoiceNumber, sale.InvoiceNumber);
        Assert.False(sale.SoftDeleted);
        Assert.NotEqual(Guid.Empty, sale.SaleId);
        
        Assert.Single(result.Data.SaleItems);
        Assert.Equal(saleItem, result.Data.SaleItems.First());
    }
    [Fact]
    public void AddMultipleSaleItems_ShouldBeAdded_InputValidSaleItems()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        Result<Sale> result = Sale.Create(outletId, invoiceNumber);
        SaleItem saleItem = SaleItem.Create(Guid.NewGuid(), result.Data.SaleId, 5, 10.0m, 0.2m, "Sample remark", 0.05m).Data;
        SaleItem saleItem2 = SaleItem.Create(Guid.NewGuid(), result.Data.SaleId, 5, 7.2m, 0.0m, "Sample remark2", 0.0m).Data;

        // Act
        var resultAddingSaleItem = result.Data.AddSaleItem(saleItem);
        var resultAddingSaleItem2 = result.Data.AddSaleItem(saleItem2);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var sale = result.Data;
        Assert.Equal(outletId, sale.OutletId);
        Assert.Equal(invoiceNumber, sale.InvoiceNumber);
        Assert.False(sale.SoftDeleted);
        Assert.NotEqual(Guid.Empty, sale.SaleId);

        Assert.True(resultAddingSaleItem.IsSuccess);
        Assert.True(resultAddingSaleItem2.IsSuccess);
        Assert.Equal(2, sale.SaleItems.Count);
        Assert.Equal(2, result.Data.SaleItems.Count);
        Assert.Equal(saleItem, result.Data.SaleItems.FirstOrDefault(d => d.SaleItemId == saleItem.SaleItemId));
        Assert.Equal(saleItem2, result.Data.SaleItems.FirstOrDefault(d => d.SaleItemId == saleItem2.SaleItemId));
        Assert.Equal(78M, result.Data.TotalAmount);
        Assert.Equal(76M,result.Data.NetAmount);
    }

    [Fact]
    public void UpdateSaleItem_ShouldBeUpdated_InputValidUpdatedSaleItem()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        Result<Sale> result = Sale.Create(outletId, invoiceNumber);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var sale = result.Data;

        var productId = Guid.NewGuid();
        SaleItem saleItem = SaleItem.Create(productId, sale.SaleId, 5, 10.0m, 0.15m, "Sample remark", 0.05m).Data!;
        var resultAddingSaleItem = sale.AddSaleItem(saleItem);
        Assert.True(resultAddingSaleItem.IsSuccess);

        // Act
        var resultUpdatingSaleItem = sale.UpdateSaleItem(
            saleItem.SaleItemId, 
            quantity: 5, 
            unitPrice: 7.2m, 
            unitDiscount: 0.0m, 
            remark: "Sample remark2", 
            taxRate: 0.0m);

        // Assert
        Assert.True(resultUpdatingSaleItem.IsSuccess);
        var updatedItem = sale.SaleItems.FirstOrDefault(d => d.SaleItemId == saleItem.SaleItemId);
        Assert.NotNull(updatedItem);
        Assert.Equal(7.2m, updatedItem.UnitPrice);
        Assert.Equal(0.0m, updatedItem.UnitDiscount);
        Assert.Equal("Sample remark2", updatedItem.Remark);
        Assert.Equal(0.0m, updatedItem.TaxRate);
    }
}
