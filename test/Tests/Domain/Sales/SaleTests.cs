using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using Xunit;

namespace Tests.Domain.Sales;

public class SaleTests
{
    private static readonly Guid CompanyId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Sale_Should_Have_Correct_Properties()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";

        // Act
        Result<Sale> result = Sale.Create(outletId, UserId, invoiceNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var sale = result.Data;

        Assert.Equal(outletId, sale.OutletId);
        Assert.Equal(UserId, sale.CreatedByUserId);
        Assert.Equal(invoiceNumber, sale.InvoiceNumber);
        Assert.False(sale.SoftDeleted);
        Assert.Null(sale.DateTimeSoftDeleted);
        Assert.False(sale.Void);
        Assert.Null(sale.VoidedByUserId);
        Assert.Null(sale.DateTimeVoided);
        Assert.NotEqual(Guid.Empty, sale.SaleId);
    }

    [Fact]
    public void AddSaleItem_ShouldBeAdded_InputValidSaleItem()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        Result<Sale> result = Sale.Create(outletId, UserId, invoiceNumber);
        Assert.NotNull(result.Data);

        var taxes = new List<Tax> { Tax.Create(CompanyId, "Tax", 0.05m).Data! };
        SaleItem saleItem = SaleItem.Create(Guid.NewGuid(), result.Data.SaleId, UserId, 5, 10.0m, 1.0m, "Sample remark", taxes).Data!;

        // Act
        var resultAddingSaleItem = result.Data.AddSaleItem(saleItem);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var sale = result.Data;
        Assert.True(resultAddingSaleItem.IsSuccess);
        Assert.Single(sale.SaleItems);
        Assert.Equal(outletId, sale.OutletId);
        Assert.Equal(UserId, sale.CreatedByUserId);
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
        Result<Sale> result = Sale.Create(outletId, UserId, invoiceNumber);
        Assert.NotNull(result.Data);

        var taxes1 = new List<Tax> { Tax.Create(CompanyId, "Tax1", 0.05m).Data! };
        SaleItem saleItem = SaleItem.Create(Guid.NewGuid(), result.Data.SaleId, UserId, 5, 10.0m, 0.2m, "Sample remark", taxes1).Data!;
        SaleItem saleItem2 = SaleItem.Create(Guid.NewGuid(), result.Data.SaleId, UserId, 5, 7.2m, 0.0m, "Sample remark2", taxes: null).Data!;

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
        Assert.Equal(76M, result.Data.NetAmount);
    }

    [Fact]
    public void UpdateSaleItem_ShouldBeUpdated_InputValidUpdatedSaleItem()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        Result<Sale> result = Sale.Create(outletId, UserId, invoiceNumber);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var sale = result.Data;

        var productId = Guid.NewGuid();
        var taxes = new List<Tax> { Tax.Create(CompanyId, "Tax1", 0.05m).Data! };
        SaleItem saleItem = SaleItem.Create(productId, sale.SaleId, UserId, 5, 10.0m, 0.15m, "Sample remark", taxes).Data!;
        var resultAddingSaleItem = sale.AddSaleItem(saleItem);
        Assert.True(resultAddingSaleItem.IsSuccess);

        // Act
        var resultUpdatingSaleItem = sale.UpdateSaleItem(
            saleItem.SaleItemId, 
            quantity: 5, 
            unitPrice: 7.2m, 
            unitDiscount: 0.0m, 
            remark: "Sample remark2", 
            taxes: null);

        // Assert
        Assert.True(resultUpdatingSaleItem.IsSuccess);
        var updatedItem = sale.SaleItems.FirstOrDefault(d => d.SaleItemId == saleItem.SaleItemId);
        Assert.NotNull(updatedItem);
        Assert.Equal(7.2m, updatedItem.UnitPrice);
        Assert.Equal(0.0m, updatedItem.UnitDiscount);
        Assert.Equal("Sample remark2", updatedItem.Remark);
        Assert.Equal(0.0m, updatedItem.TaxRate);
        Assert.Empty(updatedItem.SaleItemTaxes);
    }

    [Fact]
    public void VoidSale_ShouldSetVoidFlags_AndVoidedByUserId()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var voidingUserId = Guid.NewGuid();

        // Act
        var result = sale.VoidSale(voidingUserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(sale.Void);
        Assert.Equal(voidingUserId, sale.VoidedByUserId);
        Assert.NotNull(sale.DateTimeVoided);
    }

    [Fact]
    public void VoidSaleItem_ShouldExcludeFromTotals()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;

        var item1 = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 2, 50m, 0m, "Item 1").Data!;
        var item2 = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 30m, 0m, "Item 2").Data!;
        sale.AddSaleItem(item1);
        sale.AddSaleItem(item2);

        Assert.Equal(130m, sale.TotalAmount);

        // Act
        var voidingUserId = Guid.NewGuid();
        var voidResult = sale.VoidSaleItem(item2.SaleItemId, voidingUserId);

        // Assert
        Assert.True(voidResult.IsSuccess);
        Assert.True(item2.Void);
        Assert.Equal(voidingUserId, item2.VoidedByUserId);
        Assert.NotNull(item2.DateTimeVoided);
        Assert.Equal(100m, sale.TotalAmount); // item2 (30m) excluded from total!
    }

    [Fact]
    public void SoftDelete_ShouldReturnSuccess_AndSetFlags()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var beforeDelete = DateTime.UtcNow;

        // Act
        Result result = sale.SoftDelete();
        var afterDelete = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(sale.SoftDeleted);
        Assert.NotNull(sale.DateTimeSoftDeleted);
        Assert.InRange(sale.DateTimeSoftDeleted.Value, beforeDelete, afterDelete);
    }
}
