using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Common;
using SimplePos.Application.Products.Queries.ProductListByCompanyId;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;

namespace SimplePos.Application.Products.Queries.ProductListByCompanyId;

public class GetProductByCompanyIdQueryHandler : IQueryHandler<GetProductByCompanyIdQuery, Result<PagedList<ProductListItemResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ITaxRepository _taxRepository;

    public GetProductByCompanyIdQueryHandler(IProductRepository productRepository, ITaxRepository taxRepository)
    {
        _productRepository = productRepository;
        _taxRepository = taxRepository;
    }

    public async Task<Result<PagedList<ProductListItemResponse>>> HandleAsync(GetProductByCompanyIdQuery query, CancellationToken cancellationToken)
    {
        List<Tax> taxResult = await _taxRepository.GetTaxesByCompanyIdAsync(query.CompanyId);
        (List<Product> productsResult, int Count) = await _productRepository.GetPagedProductsByCompanyIdAsync(query.CompanyId, query.Page, query.PageSize, query.SortBy, query.IsDescending);

        List<ProductListItemResponse> ProductResponses = productsResult.Where(p => !p.SoftDeleted).Select(p =>
        {
            //decimal totalTaxPercentage = p.ProductTaxes.Sum(pt => taxResult.FirstOrDefault(t => t.TaxId == pt.TaxId)?.TaxRate ?? 0m);
            decimal totalTaxPercentage = p.ProductTaxes.Sum(pt => taxResult.FirstOrDefault(t => t.TaxId == pt.TaxId)?.TaxRate ?? 0m);
            decimal priceWithTax = p.BasePrice + (p.BasePrice * totalTaxPercentage);
            return new ProductListItemResponse(
                p.ProductId,
                p.CategoryId,
                p.SKU,
                p.ProductName,
                p.BasePrice,
                p.IsActive,
                totalTaxPercentage,
                priceWithTax
            );
        }).ToList();

        int pages = (int)Math.Ceiling((double)Count / query.PageSize);

        PagedList<ProductListItemResponse> pagedListResponse = new PagedList<ProductListItemResponse>(ProductResponses, query.Page, query.PageSize, Count, pages);
        return Result<PagedList<ProductListItemResponse>>.Success(pagedListResponse);
    }
}