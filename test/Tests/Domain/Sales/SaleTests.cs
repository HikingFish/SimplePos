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
        Assert.False(sale.Closed);
        Assert.Null(sale.ClosedByUserId);
        Assert.Null(sale.DateTimeClosed);
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

    [Fact]
    public void CloseSale_ShouldSetClosedFlags_AndClosedByUserId()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var closingUserId = Guid.NewGuid();
        var beforeClose = DateTime.UtcNow;

        // Act
        var result = sale.CloseSale(closingUserId);
        var afterClose = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(sale.Closed);
        Assert.Equal(closingUserId, sale.ClosedByUserId);
        Assert.NotNull(sale.DateTimeClosed);
        Assert.InRange(sale.DateTimeClosed.Value, beforeClose, afterClose);
    }

    [Fact]
    public void CloseSale_ShouldFail_WhenSaleAlreadyClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        sale.CloseSale(Guid.NewGuid());

        // Act
        var result = sale.CloseSale(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void CloseSale_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;

        // Act
        var result = sale.CloseSale(Guid.Empty);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.ClosedByUserIdEmpty, result.Error);
    }

    [Fact]
    public void CloseSale_ShouldFail_WhenSaleIsVoid()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        sale.VoidSale(UserId);

        // Act
        var result = sale.CloseSale(UserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Void, result.Error);
    }

    [Fact]
    public void CloseSale_ShouldFail_WhenSaleIsSoftDeleted()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        sale.SoftDelete();

        // Act
        var result = sale.CloseSale(UserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.SoftDeleted, result.Error);
    }

    [Fact]
    public void AddSaleItem_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        sale.CloseSale(UserId);
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 10m, 0m, "Item").Data!;

        // Act
        var result = sale.AddSaleItem(saleItem);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void AddPayment_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var item = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 100m, 0m, "Item").Data!;
        sale.AddSaleItem(item);
        sale.CloseSale(UserId);

        var payment = SalePayment.Create(sale.SaleId, Guid.NewGuid(), UserId, 50m).Data!;

        // Act
        var result = sale.AddPayment(payment);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void RemoveSaleItem_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 10m, 0m, "Item").Data!;
        sale.AddSaleItem(saleItem);
        sale.CloseSale(UserId);

        // Act
        var result = sale.RemoveSaleItem(saleItem.SaleItemId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void RemovePayment_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var item = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 100m, 0m, "Item").Data!;
        sale.AddSaleItem(item);
        var payment = SalePayment.Create(sale.SaleId, Guid.NewGuid(), UserId, 50m).Data!;
        sale.AddPayment(payment);
        sale.CloseSale(UserId);

        // Act
        var result = sale.RemovePayment(payment.SalePaymentId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void UpdateSaleItem_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 10m, 0m, "Item").Data!;
        sale.AddSaleItem(saleItem);
        sale.CloseSale(UserId);

        // Act
        var result = sale.UpdateSaleItem(saleItem.SaleItemId, 2, 15m, 0m, "Updated");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void VoidSale_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        sale.CloseSale(UserId);

        // Act
        var result = sale.VoidSale(UserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void UnvoidSale_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        // Direct close (cannot close a void sale, but testing guard against closed state)
        sale.CloseSale(UserId);

        // Act
        var result = sale.UnvoidSale();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void VoidSaleItem_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 10m, 0m, "Item").Data!;
        sale.AddSaleItem(saleItem);
        sale.CloseSale(UserId);

        // Act
        var result = sale.VoidSaleItem(saleItem.SaleItemId, UserId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void UnvoidSaleItem_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 1, 10m, 0m, "Item").Data!;
        sale.AddSaleItem(saleItem);
        sale.VoidSaleItem(saleItem.SaleItemId, UserId);
        sale.CloseSale(UserId);

        // Act
        var result = sale.UnvoidSaleItem(saleItem.SaleItemId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void EditSaleItemQuantity_ShouldUpdateQuantityAndRecalculateTotals()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 2, 50m, 0m, "Item").Data!;
        sale.AddSaleItem(saleItem);
        Assert.Equal(100m, sale.TotalAmount);

        // Act
        var result = sale.EditSaleItemQuantity(saleItem.SaleItemId, 4);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4, saleItem.Quantity);
        Assert.Equal(200m, sale.TotalAmount);
        Assert.Equal(200m, sale.TotalOutstanding);
    }

    [Fact]
    public void EditSaleItemQuantity_ShouldFail_WhenSaleIsClosed()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 2, 50m, 0m, "Item").Data!;
        sale.AddSaleItem(saleItem);
        sale.CloseSale(UserId);

        // Act
        var result = sale.EditSaleItemQuantity(saleItem.SaleItemId, 4);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.Closed, result.Error);
    }

    [Fact]
    public void EditSaleItemQuantity_ShouldFail_WhenSaleItemNotFound()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;

        // Act
        var result = sale.EditSaleItemQuantity(Guid.NewGuid(), 4);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.SaleItemNotFound, result.Error);
    }

    [Fact]
    public void EditSaleItemQuantity_ShouldFail_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var invoiceNumber = "INV-001";
        var sale = Sale.Create(outletId, UserId, invoiceNumber).Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, UserId, 2, 50m, 0m, "Item").Data!;
        sale.AddSaleItem(saleItem);

        // Act
        var result = sale.EditSaleItemQuantity(saleItem.SaleItemId, 0);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleItemError.QuantityZero, result.Error);
    }
}
