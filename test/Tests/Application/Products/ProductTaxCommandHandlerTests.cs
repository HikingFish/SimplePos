using NSubstitute;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Products.Commands.AddProductTax;
using SimplePos.Application.Products.Commands.RemoveProductTax;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Products;

public class ProductTaxCommandHandlerTests
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly ITaxRepository _taxRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public ProductTaxCommandHandlerTests()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _taxRepositoryMock = Substitute.For<ITaxRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task AddProductTax_ShouldReturnSuccess_WhenProductAndTaxExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var product = Product.Create(companyId, null, "SKU-1", "Product 1", 10m, 20m).Data!;
        var tax = Tax.Create(companyId, "VAT", 0.10m).Data!;

        _productRepositoryMock.GetProductByIdAsync(product.ProductId).Returns(product);
        _taxRepositoryMock.GetTaxByIdAsync(tax.TaxId).Returns(tax);

        var handler = new AddProductTaxCommandHandler(_productRepositoryMock, _taxRepositoryMock, _unitOfWorkMock);
        var command = new AddProductTaxCommand(product.ProductId, companyId, tax.TaxId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(product.ProductTaxes);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddProductTax_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _productRepositoryMock.GetProductByIdAsync(Arg.Any<Guid>()).Returns((Product?)null);

        var handler = new AddProductTaxCommandHandler(_productRepositoryMock, _taxRepositoryMock, _unitOfWorkMock);
        var command = new AddProductTaxCommand(Guid.NewGuid(), companyId, Guid.NewGuid());

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ProductError.ProductNotExist.Code, result.Error.Code);
    }

    [Fact]
    public async Task AddProductTax_ShouldReturnNotFound_WhenTaxDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var product = Product.Create(companyId, null, "SKU-1", "Product 1", 10m, 20m).Data!;
        _productRepositoryMock.GetProductByIdAsync(product.ProductId).Returns(product);
        _taxRepositoryMock.GetTaxByIdAsync(Arg.Any<Guid>()).Returns((Tax?)null);

        var handler = new AddProductTaxCommandHandler(_productRepositoryMock, _taxRepositoryMock, _unitOfWorkMock);
        var command = new AddProductTaxCommand(product.ProductId, companyId, Guid.NewGuid());

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaxError.NotExist.Code, result.Error.Code);
    }

    [Fact]
    public async Task RemoveProductTax_ShouldReturnSuccess_WhenTaxIsAssociated()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var product = Product.Create(companyId, null, "SKU-1", "Product 1", 10m, 20m).Data!;
        var taxId = Guid.NewGuid();
        product.AddProductTax(taxId);

        _productRepositoryMock.GetProductByIdAsync(product.ProductId).Returns(product);

        var handler = new RemoveProductTaxCommandHandler(_productRepositoryMock, _unitOfWorkMock);
        var command = new RemoveProductTaxCommand(product.ProductId, companyId, taxId);

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(product.ProductTaxes);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveProductTax_ShouldReturnNotFound_WhenTaxNotAssociated()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var product = Product.Create(companyId, null, "SKU-1", "Product 1", 10m, 20m).Data!;

        _productRepositoryMock.GetProductByIdAsync(product.ProductId).Returns(product);

        var handler = new RemoveProductTaxCommandHandler(_productRepositoryMock, _unitOfWorkMock);
        var command = new RemoveProductTaxCommand(product.ProductId, companyId, Guid.NewGuid());

        // Act
        Result result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ProductError.TaxNotAssociated.Code, result.Error.Code);
    }
}
