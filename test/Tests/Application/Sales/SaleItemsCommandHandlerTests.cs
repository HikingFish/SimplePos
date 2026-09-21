using NSubstitute;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Sales.Commands.AddSaleItem;
using SimplePos.Application.Sales.Commands.RemoveSaleItem;
using SimplePos.Application.Sales.Commands.UnvoidSaleItem;
using SimplePos.Application.Sales.Commands.UpdateSaleItem;
using SimplePos.Application.Sales.Commands.UpdateSaleItemQuantity;
using SimplePos.Application.Sales.Commands.VoidSaleItem;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Sales;

public class SaleItemsCommandHandlerTests
{
    private readonly ISaleRepository _saleRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IOutletProductAvailabilityRepository _outletAvailabilityMock;
    private readonly ITaxRepository _taxRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public SaleItemsCommandHandlerTests()
    {
        _saleRepositoryMock = Substitute.For<ISaleRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _outletAvailabilityMock = Substitute.For<IOutletProductAvailabilityRepository>();
        _taxRepositoryMock = Substitute.For<ITaxRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task AddSaleItem_ShouldReturnSuccess_WhenItemIsValid()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var product = Product.Create(companyId, Guid.NewGuid(), "SKU1", "Coffee", 5m, 10m).Data!;
        _productRepositoryMock.GetProductByIdAsync(product.ProductId).Returns(product);
        _outletAvailabilityMock.GetByOutletAndProductIdAsync(outletId, product.ProductId).Returns((OutletProductAvailability?)null);
        _taxRepositoryMock.GetActiveTaxesByCompanyIdAsync(companyId).Returns(new List<Tax>());

        var handler = new AddSaleItemCommandHandler(
            _saleRepositoryMock,
            _productRepositoryMock,
            _outletAvailabilityMock,
            _taxRepositoryMock,
            _unitOfWorkMock);

        var command = new AddSaleItemCommand(
            sale.SaleId,
            userId,
            companyId,
            product.ProductId,
            2,
            10m,
            0,
            "Extra sugar",
            DateTime.UtcNow);

        // Act
        Result<Guid> result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(sale.SaleItems);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditSaleItemQuantity_ShouldReturnSuccess_WhenItemExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 10m, 0, null).Data!;
        sale.AddSaleItem(saleItem);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new UpdateSaleItemQuantityCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new UpdateSaleItemQuantityCommand(sale.SaleId, saleItem.SaleItemId, 5);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, saleItem.Quantity);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task VoidSaleItem_ShouldReturnSuccess_WhenItemExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 10m, 0, null).Data!;
        sale.AddSaleItem(saleItem);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new VoidSaleItemCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new VoidSaleItemCommand(sale.SaleId, saleItem.SaleItemId, userId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(saleItem.Void);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UnvoidSaleItem_ShouldReturnSuccess_WhenItemIsVoided()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 10m, 0, null).Data!;
        sale.AddSaleItem(saleItem);
        sale.VoidSaleItem(saleItem.SaleItemId, userId);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new UnvoidSaleItemCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new UnvoidSaleItemCommand(sale.SaleId, saleItem.SaleItemId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(saleItem.Void);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveSaleItem_ShouldReturnSuccess_WhenItemExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 10m, 0, null).Data!;
        sale.AddSaleItem(saleItem);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new RemoveSaleItemCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new RemoveSaleItemCommand(sale.SaleId, saleItem.SaleItemId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(sale.SaleItems);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateSaleItem_ShouldReturnSuccess_WhenItemExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 10m, 0, null).Data!;
        sale.AddSaleItem(saleItem);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new UpdateSaleItemCommandHandler(_saleRepositoryMock, _taxRepositoryMock, _unitOfWorkMock);
        var command = new UpdateSaleItemCommand(sale.SaleId, saleItem.SaleItemId, 3, 15m, 0.1m, "Updated remark", null, companyId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, saleItem.Quantity);
        Assert.Equal(15m, saleItem.UnitPrice);
        Assert.Equal(0.1m, saleItem.UnitDiscount);
        Assert.Equal("Updated remark", saleItem.Remark);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
