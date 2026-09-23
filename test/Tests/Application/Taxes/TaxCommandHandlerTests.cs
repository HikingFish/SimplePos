using NSubstitute;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Taxes.Commands.ActivateTax;
using SimplePos.Application.Taxes.Commands.CreateTax;
using SimplePos.Application.Taxes.Commands.DeactivateTax;
using SimplePos.Application.Taxes.Commands.DeleteTax;
using SimplePos.Application.Taxes.Commands.UpdateTax;
using SimplePos.Application.Taxes.Common;
using SimplePos.Application.Taxes.Queries.GetActiveTaxesByCompany;
using SimplePos.Application.Taxes.Queries.GetTaxById;
using SimplePos.Application.Taxes.Queries.GetTaxesByCompany;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Taxes;

public class TaxCommandHandlerTests
{
    private readonly ITaxRepository _taxRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public TaxCommandHandlerTests()
    {
        _taxRepositoryMock = Substitute.For<ITaxRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task CreateTax_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var handler = new CreateTaxCommandHandler(_taxRepositoryMock, _unitOfWorkMock);
        var command = new CreateTaxCommand(companyId, "VAT 10%", 0.10m);

        // Act
        Result<Guid> result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _taxRepositoryMock.Received(1).AddTax(Arg.Is<Tax>(t => t != null && t.TaxName == "VAT 10%" && t.TaxRate == 0.10m));
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateTax_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var handler = new CreateTaxCommandHandler(_taxRepositoryMock, _unitOfWorkMock);
        var command = new CreateTaxCommand(companyId, "", 0.10m);

        // Act
        Result<Guid> result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaxError.TaxNameEmpty.Code, result.Error.Code);
        _taxRepositoryMock.DidNotReceive().AddTax(Arg.Any<Tax>());
    }

    [Fact]
    public async Task UpdateTax_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var tax = Tax.Create(companyId, "Standard Tax", 0.05m).Data!;
        _taxRepositoryMock.GetTaxByIdAsync(tax.TaxId).Returns(tax);

        var handler = new UpdateTaxCommandHandler(_taxRepositoryMock, _unitOfWorkMock);
        var command = new UpdateTaxCommand(tax.TaxId, companyId, "Updated Tax", 0.08m);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Tax", tax.TaxName);
        Assert.Equal(0.08m, tax.TaxRate);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTax_ShouldReturnNotFound_WhenTaxDoesNotExist()
    {
        // Arrange
        _taxRepositoryMock.GetTaxByIdAsync(Arg.Any<Guid>()).Returns((Tax?)null);

        var handler = new UpdateTaxCommandHandler(_taxRepositoryMock, _unitOfWorkMock);
        var command = new UpdateTaxCommand(Guid.NewGuid(), Guid.NewGuid(), "Tax", 0.10m);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaxError.NotExist.Code, result.Error.Code);
    }

    [Fact]
    public async Task ActivateTax_ShouldReturnSuccess_WhenInactive()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var tax = Tax.Create(companyId, "GST", 0.07m).Data!;
        tax.Deactivate();
        _taxRepositoryMock.GetTaxByIdAsync(tax.TaxId).Returns(tax);

        var handler = new ActivateTaxCommandHandler(_taxRepositoryMock, _unitOfWorkMock);
        var command = new ActivateTaxCommand(tax.TaxId, companyId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(tax.IsActive);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeactivateTax_ShouldReturnSuccess_WhenActive()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var tax = Tax.Create(companyId, "GST", 0.07m).Data!;
        _taxRepositoryMock.GetTaxByIdAsync(tax.TaxId).Returns(tax);

        var handler = new DeactivateTaxCommandHandler(_taxRepositoryMock, _unitOfWorkMock);
        var command = new DeactivateTaxCommand(tax.TaxId, companyId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(tax.IsActive);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTax_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var tax = Tax.Create(companyId, "GST", 0.07m).Data!;
        _taxRepositoryMock.GetTaxByIdAsync(tax.TaxId).Returns(tax);

        var handler = new DeleteTaxCommandHandler(_taxRepositoryMock, _unitOfWorkMock);
        var command = new DeleteTaxCommand(tax.TaxId, companyId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(tax.SoftDeleted);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTaxById_ShouldReturnTax_WhenExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var tax = Tax.Create(companyId, "GST", 0.07m).Data!;
        _taxRepositoryMock.GetTaxByIdAsync(tax.TaxId).Returns(tax);

        var handler = new GetTaxByIdQueryHandler(_taxRepositoryMock);
        var query = new GetTaxByIdQuery(tax.TaxId, companyId);

        // Act
        Result<TaxResponse> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tax.TaxId, result.Data!.TaxId);
        Assert.Equal("GST", result.Data.TaxName);
        Assert.Equal(0.07m, result.Data.TaxRate);
    }

    [Fact]
    public async Task GetTaxesByCompany_ShouldReturnAllNonDeletedTaxes()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var tax1 = Tax.Create(companyId, "Tax 1", 0.05m).Data!;
        var tax2 = Tax.Create(companyId, "Tax 2", 0.10m).Data!;
        var tax3 = Tax.Create(companyId, "Tax 3", 0.15m).Data!;
        tax3.SoftDelete();

        _taxRepositoryMock.GetTaxesByCompanyIdAsync(companyId).Returns(new List<Tax> { tax1, tax2, tax3 });

        var handler = new GetTaxesByCompanyQueryHandler(_taxRepositoryMock);
        var query = new GetTaxesByCompanyQuery(companyId);

        // Act
        Result<List<TaxResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetActiveTaxesByCompany_ShouldReturnOnlyActiveTaxes()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var tax1 = Tax.Create(companyId, "Tax 1", 0.05m).Data!;
        var tax2 = Tax.Create(companyId, "Tax 2", 0.10m).Data!;
        tax2.Deactivate();

        _taxRepositoryMock.GetActiveTaxesByCompanyIdAsync(companyId).Returns(new List<Tax> { tax1 });

        var handler = new GetActiveTaxesByCompanyQueryHandler(_taxRepositoryMock);
        var query = new GetActiveTaxesByCompanyQuery(companyId);

        // Act
        Result<List<TaxResponse>> result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("Tax 1", result.Data[0].TaxName);
    }
}
