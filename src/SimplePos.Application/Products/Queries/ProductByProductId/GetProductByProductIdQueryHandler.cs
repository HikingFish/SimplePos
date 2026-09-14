using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Queries.ProductByProductId;

public class GetProductByProductIdQueryHandler : IQueryHandler<GetProductByProductIdQuery, Result<ProductResponse>>
{
    private readonly ITaxRepository _taxRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetProductByProductIdQueryHandler(ITaxRepository taxRepository, IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _taxRepository = taxRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<ProductResponse>> HandleAsync(GetProductByProductIdQuery query, CancellationToken cancellationToken)
    {
        Product? productResult = await _productRepository.GetProductByIdAsync(query.ProductId);

        if (productResult == null || productResult.CompanyId != query.CompanyId || productResult.SoftDeleted)
        {
            return Result<ProductResponse>.Failure(ProductError.ProductNotExist);
        }

        var taxIds = productResult.ProductTaxes.Select(pt => pt.TaxId);
        List<Tax> taxes = await _taxRepository.GetTaxesByIdsAsync(taxIds);

        string categoryName = string.Empty;
        if (productResult.CategoryId.HasValue)
        {
            Category? category = await _categoryRepository.GetCategoryByIdAsync(productResult.CategoryId.Value);
            categoryName = category?.Name ?? string.Empty;
        }

        ProductResponse response = new ProductResponse(
            productResult.ProductId,
            productResult.CategoryId,
            productResult.SKU,
            productResult.ProductName,
            productResult.CostPrice,
            productResult.BasePrice,
            productResult.IsActive,
            categoryName,
            taxes
        );

        return Result<ProductResponse>.Success(response);
    }
}
