using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Common;
using SimplePos.Application.Products.Queries.ProductListByCompanyId;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;

namespace SimplePos.Application.Products.Queries.ProductListByCompanyId;

public class GetProductByCompanyIdQueryHandler : IQueryHandler<GetProductByCompanyIdQuery, Result<PagedList<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ITaxRepository _taxRepository;

    public GetProductByCompanyIdQueryHandler(IProductRepository productRepository, ITaxRepository taxRepository)
    {
        _productRepository = productRepository;
        _taxRepository = taxRepository;
    }

    public async Task<Result<PagedList<ProductResponse>>> HandleAsync(GetProductByCompanyIdQuery query, CancellationToken cancellationToken)
    {
        List<Tax> taxResult = await _taxRepository.GetTaxesByCompanyIdAsync(query.CompanyId);
        (List<Product> productsResult, int Count) = await _productRepository.GetPagedProductsByCompanyIdAsync(query.CompanyId, query.Page, query.PageSize, query.SortBy, query.IsDescending);

        List<ProductResponse> ProductResponses = productsResult.Select
    }
}