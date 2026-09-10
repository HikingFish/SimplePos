using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Result<Guid>>
{
    private readonly ITaxRepository _taxRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(ITaxRepository taxRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _taxRepository = taxRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (command.CategoryId.HasValue)
        {
            Category? categoryExists = await _categoryRepository.GetCategoryByIdAsync(command.CategoryId.Value);
            if (categoryExists == null)
            {
                return Result<Guid>.Failure(CategoryError.NotExist);
            }
        }

        Result<Product> productResult = Product.Create(command.CompanyId, command.CategoryId, command.SKU, command.ProductName, command.CostPrice, command.BasePrice);
        if (productResult.Data == null)
        {
            throw new InvalidOperationException("Product creation failed, product data is null.");
        }

        foreach (Guid taxId in command.Taxes)
        {
            var tax = await _taxRepository.GetTaxByIdAsync(taxId);
            if (tax is null)
            {
                return Result<Guid>.Failure(TaxError.NotExist);
            }

            productResult.Data.AddProductTax(tax.TaxId);
        }

        if (!productResult.IsSuccess)
        {
            return Result<Guid>.Failure(productResult.Error);
        }

        _productRepository.AddProduct(productResult.Data);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(productResult.Data.ProductId);
    }
}