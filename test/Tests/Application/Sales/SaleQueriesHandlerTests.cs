using NSubstitute;
using SimplePos.Application.Sales.Common;
using SimplePos.Application.Sales.Queries.GetSaleById;
using SimplePos.Application.Sales.Queries.GetSalePayments;
using SimplePos.Application.Sales.Queries.GetSales;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Sales;

public class SaleQueriesHandlerTests
{
    private readonly ISaleRepository _saleRepositoryMock;

    public SaleQueriesHandlerTests()
    {
        _saleRepositoryMock = Substitute.For<ISaleRepository>();
    }

    [Fact]
    public async Task GetSaleById_ShouldReturnSaleResponse_WhenSaleExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new GetSaleByIdQueryHandler(_saleRepositoryMock);
        var query = new GetSaleByIdQuery(sale.SaleId);

        // Act
        Result<SaleResponse> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(sale.SaleId, result.Data.SaleId);
        Assert.Equal("INV-001", result.Data.InvoiceNumber);
    }

    [Fact]
    public async Task GetSaleById_ShouldReturnNotFound_WhenSaleDoesNotExist()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepositoryMock.GetSaleByIdAsync(saleId).Returns((Sale?)null);

        var handler = new GetSaleByIdQueryHandler(_saleRepositoryMock);
        var query = new GetSaleByIdQuery(saleId);

        // Act
        Result<SaleResponse> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(SaleError.SaleNotFound.Code, result.Error.Code);
    }

    [Fact]
    public async Task GetSales_ShouldReturnList_WhenFilteredByOutlet()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSalesByOutletIdAsync(outletId).Returns(new List<Sale> { sale });

        var handler = new GetSalesQueryHandler(_saleRepositoryMock);
        var query = new GetSalesQuery(outletId, null, null, null);

        // Act
        Result<List<SaleResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
    }

    [Fact]
    public async Task GetSales_ShouldReturnList_WhenFilteredByDateRange()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var from = DateTime.UtcNow.AddDays(-1);
        var to = DateTime.UtcNow.AddDays(1);
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSalesByDateRangeAsync(outletId, from, to).Returns(new List<Sale> { sale });

        var handler = new GetSalesQueryHandler(_saleRepositoryMock);
        var query = new GetSalesQuery(outletId, from, to, null);

        // Act
        Result<List<SaleResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
    }

    [Fact]
    public async Task GetSales_ShouldReturnList_WhenFilteredByUser()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        _saleRepositoryMock.GetSalesByUserIdAsync(userId).Returns(new List<Sale> { sale });

        var handler = new GetSalesQueryHandler(_saleRepositoryMock);
        var query = new GetSalesQuery(null, null, null, userId);

        // Act
        Result<List<SaleResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
    }

    [Fact]
    public async Task GetSalePayments_ShouldReturnPayments_WhenSaleExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var item = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 100m, 0, null).Data!;
        sale.AddSaleItem(item);
        var payment = SalePayment.Create(sale.SaleId, Guid.NewGuid(), userId, 100m).Data!;
        sale.AddPayment(payment);

        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new GetSalePaymentsQueryHandler(_saleRepositoryMock);
        var query = new GetSalePaymentsQuery(sale.SaleId);

        // Act
        Result<List<SalePaymentResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
        Assert.Equal(100m, result.Data![0].AmountPaid);
    }
}
