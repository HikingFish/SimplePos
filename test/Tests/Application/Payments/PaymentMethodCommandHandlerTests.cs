using NSubstitute;
using SimplePos.Application.Abstractions;
using SimplePos.Application.PaymentMethods.Commands.ActivatePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.CreatePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.DeactivatePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.DeletePaymentMethod;
using SimplePos.Application.PaymentMethods.Commands.UpdatePaymentMethod;
using SimplePos.Application.PaymentMethods.Common;
using SimplePos.Application.PaymentMethods.Queries.GetActivePaymentMethodsByCompany;
using SimplePos.Application.PaymentMethods.Queries.GetPaymentMethodById;
using SimplePos.Application.PaymentMethods.Queries.GetPaymentMethodsByCompany;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Payments;

public class PaymentMethodCommandHandlerTests
{
    private readonly IPaymentMethodRepository _paymentMethodRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public PaymentMethodCommandHandlerTests()
    {
        _paymentMethodRepositoryMock = Substitute.For<IPaymentMethodRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task CreatePaymentMethod_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var handler = new CreatePaymentMethodCommandHandler(_paymentMethodRepositoryMock, _unitOfWorkMock);
        var command = new CreatePaymentMethodCommand(companyId, "Credit Card");

        // Act
        Result<Guid> result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _paymentMethodRepositoryMock.Received(1).AddPaymentMethod(Arg.Is<PaymentMethod>(pm => pm != null && pm.Name == "Credit Card"));
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreatePaymentMethod_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var handler = new CreatePaymentMethodCommandHandler(_paymentMethodRepositoryMock, _unitOfWorkMock);
        var command = new CreatePaymentMethodCommand(companyId, "");

        // Act
        Result<Guid> result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentMethodError.NameEmpty.Code, result.Error.Code);
        _paymentMethodRepositoryMock.DidNotReceive().AddPaymentMethod(Arg.Any<PaymentMethod>());
    }

    [Fact]
    public async Task UpdatePaymentMethod_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var pm = PaymentMethod.Create(companyId, "Cash").Data!;
        _paymentMethodRepositoryMock.GetPaymentMethodByIdAsync(pm.PaymentMethodId).Returns(pm);

        var handler = new UpdatePaymentMethodCommandHandler(_paymentMethodRepositoryMock, _unitOfWorkMock);
        var command = new UpdatePaymentMethodCommand(pm.PaymentMethodId, companyId, "Cash (USD)");

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Cash (USD)", pm.Name);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdatePaymentMethod_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        _paymentMethodRepositoryMock.GetPaymentMethodByIdAsync(Arg.Any<Guid>()).Returns((PaymentMethod?)null);

        var handler = new UpdatePaymentMethodCommandHandler(_paymentMethodRepositoryMock, _unitOfWorkMock);
        var command = new UpdatePaymentMethodCommand(Guid.NewGuid(), Guid.NewGuid(), "E-Wallet");

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentMethodError.NotExist.Code, result.Error.Code);
    }

    [Fact]
    public async Task ActivatePaymentMethod_ShouldReturnSuccess_WhenInactive()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var pm = PaymentMethod.Create(companyId, "Crypto").Data!;
        pm.UpdateToNotActiveStatus();
        _paymentMethodRepositoryMock.GetPaymentMethodByIdAsync(pm.PaymentMethodId).Returns(pm);

        var handler = new ActivatePaymentMethodCommandHandler(_paymentMethodRepositoryMock, _unitOfWorkMock);
        var command = new ActivatePaymentMethodCommand(pm.PaymentMethodId, companyId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(pm.IsActive);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeactivatePaymentMethod_ShouldReturnSuccess_WhenActive()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var pm = PaymentMethod.Create(companyId, "Crypto").Data!;
        _paymentMethodRepositoryMock.GetPaymentMethodByIdAsync(pm.PaymentMethodId).Returns(pm);

        var handler = new DeactivatePaymentMethodCommandHandler(_paymentMethodRepositoryMock, _unitOfWorkMock);
        var command = new DeactivatePaymentMethodCommand(pm.PaymentMethodId, companyId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(pm.IsActive);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeletePaymentMethod_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var pm = PaymentMethod.Create(companyId, "Cash").Data!;
        _paymentMethodRepositoryMock.GetPaymentMethodByIdAsync(pm.PaymentMethodId).Returns(pm);

        var handler = new DeletePaymentMethodCommandHandler(_paymentMethodRepositoryMock, _unitOfWorkMock);
        var command = new DeletePaymentMethodCommand(pm.PaymentMethodId, companyId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(pm.SoftDeleted);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPaymentMethodById_ShouldReturnPaymentMethod_WhenExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var pm = PaymentMethod.Create(companyId, "Debit Card").Data!;
        _paymentMethodRepositoryMock.GetPaymentMethodByIdAsync(pm.PaymentMethodId).Returns(pm);

        var handler = new GetPaymentMethodByIdQueryHandler(_paymentMethodRepositoryMock);
        var query = new GetPaymentMethodByIdQuery(pm.PaymentMethodId, companyId);

        // Act
        Result<PaymentMethodResponse> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(pm.PaymentMethodId, result.Data!.PaymentMethodId);
        Assert.Equal("Debit Card", result.Data.Name);
    }

    [Fact]
    public async Task GetPaymentMethodsByCompany_ShouldReturnAllNonDeleted()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var pm1 = PaymentMethod.Create(companyId, "Cash").Data!;
        var pm2 = PaymentMethod.Create(companyId, "Card").Data!;
        var pm3 = PaymentMethod.Create(companyId, "Check").Data!;
        pm3.SoftDelete();

        _paymentMethodRepositoryMock.GetPaymentMethodsByCompanyIdAsync(companyId).Returns(new List<PaymentMethod> { pm1, pm2, pm3 });

        var handler = new GetPaymentMethodsByCompanyQueryHandler(_paymentMethodRepositoryMock);
        var query = new GetPaymentMethodsByCompanyQuery(companyId);

        // Act
        Result<List<PaymentMethodResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetActivePaymentMethodsByCompany_ShouldReturnActiveOnly()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var pm1 = PaymentMethod.Create(companyId, "Cash").Data!;

        _paymentMethodRepositoryMock.GetActivePaymentMethodsByCompanyIdAsync(companyId).Returns(new List<PaymentMethod> { pm1 });

        var handler = new GetActivePaymentMethodsByCompanyQueryHandler(_paymentMethodRepositoryMock);
        var query = new GetActivePaymentMethodsByCompanyQuery(companyId);

        // Act
        Result<List<PaymentMethodResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("Cash", result.Data[0].Name);
    }
}
