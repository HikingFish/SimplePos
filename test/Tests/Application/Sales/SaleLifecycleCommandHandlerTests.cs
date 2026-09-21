using NSubstitute;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Sales.Commands.CloseSale;
using SimplePos.Application.Sales.Commands.DeleteSale;
using SimplePos.Application.Sales.Commands.UnvoidSale;
using SimplePos.Application.Sales.Commands.VoidSale;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Sales;

public class SaleLifecycleCommandHandlerTests
{
    private readonly ISaleRepository _saleRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public SaleLifecycleCommandHandlerTests()
    {
        _saleRepositoryMock = Substitute.For<ISaleRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task VoidSale_ShouldReturnSuccess_WhenSaleExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new VoidSaleCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new VoidSaleCommand(sale.SaleId, userId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(sale.Void);
        Assert.Equal(userId, sale.VoidedByUserId);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task VoidSale_ShouldReturnNotFound_WhenSaleDoesNotExist()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _saleRepositoryMock.GetSaleByIdAsync(saleId).Returns((Sale?)null);

        var handler = new VoidSaleCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new VoidSaleCommand(saleId, userId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.SaleNotFound.Code, result.Error.Code);
    }

    [Fact]
    public async Task UnvoidSale_ShouldReturnSuccess_WhenSaleIsVoided()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        sale.VoidSale(userId);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new UnvoidSaleCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new UnvoidSaleCommand(sale.SaleId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(sale.Void);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CloseSale_ShouldReturnSuccess_WhenSaleIsOpen()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new CloseSaleCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new CloseSaleCommand(sale.SaleId, userId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(sale.Closed);
        Assert.Equal(userId, sale.ClosedByUserId);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteSale_ShouldReturnSuccess_WhenSaleIsNotDeleted()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new DeleteSaleCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new DeleteSaleCommand(sale.SaleId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(sale.SoftDeleted);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
