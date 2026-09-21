using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Queries.GetSales;

public class GetSalesQueryHandler : IQueryHandler<GetSalesQuery, Result<List<SaleResponse>>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<Result<List<SaleResponse>>> HandleAsync(GetSalesQuery query, CancellationToken cancellationToken)
    {
        List<Sale> sales;

        if (query.OutletId.HasValue && query.From.HasValue && query.To.HasValue)
        {
            sales = await _saleRepository.GetSalesByDateRangeAsync(query.OutletId.Value, query.From.Value, query.To.Value);
        }
        else if (query.OutletId.HasValue)
        {
            sales = await _saleRepository.GetSalesByOutletIdAsync(query.OutletId.Value);
        }
        else if (query.UserId.HasValue)
        {
            sales = await _saleRepository.GetSalesByUserIdAsync(query.UserId.Value);
        }
        else
        {
            sales = new List<Sale>();
        }

        var responses = sales.Select(s => s.ToResponse()).ToList();
        return Result<List<SaleResponse>>.Success(responses);
    }
}
