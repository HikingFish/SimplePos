using NSubstitute;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Sales.Commands.AddSalePayment;
using SimplePos.Application.Sales.Commands.RemoveSalePayment;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Sales;

public class SalePaymentsCommandHandlerTests
{
    private readonly ISaleRepository _saleRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public SalePaymentsCommandHandlerTests()
    {
        _saleRepositoryMock = Substitute.For<ISaleRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task AddSalePayment_ShouldReturnSuccess_WhenPaymentIsValid()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 100m, 0, null).Data!;
        sale.AddSaleItem(saleItem);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new AddSalePaymentCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var paymentMethodId = Guid.NewGuid();
        var command = new AddSalePaymentCommand(sale.SaleId, paymentMethodId, userId, 50m, DateTime.UtcNow, "REF-123");

        // Act
        Result<Guid> result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(sale.SalePayments);
        Assert.Equal(50m, sale.TotalPaid);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveSalePayment_ShouldReturnSuccess_WhenPaymentExists()
    {
        // Arrange
        var outletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = Sale.Create(outletId, userId, "INV-001").Data!;
        var saleItem = SaleItem.Create(Guid.NewGuid(), sale.SaleId, userId, 1, 100m, 0, null).Data!;
        sale.AddSaleItem(saleItem);

        var payment = SalePayment.Create(sale.SaleId, Guid.NewGuid(), userId, 50m).Data!;
        sale.AddPayment(payment);
        _saleRepositoryMock.GetSaleByIdAsync(sale.SaleId).Returns(sale);

        var handler = new RemoveSalePaymentCommandHandler(_saleRepositoryMock, _unitOfWorkMock);
        var command = new RemoveSalePaymentCommand(sale.SaleId, payment.SalePaymentId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(sale.SalePayments);
        Assert.Equal(0m, sale.TotalPaid);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
