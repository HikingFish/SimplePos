using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Queries.GetSaleById;

public class GetSaleByIdQueryHandler : IQueryHandler<GetSaleByIdQuery, Result<SaleResponse>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<Result<SaleResponse>> HandleAsync(GetSaleByIdQuery query, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(query.SaleId);
        if (sale == null)
        {
            return Result<SaleResponse>.Failure(SaleError.SaleNotFound);
        }

        return Result<SaleResponse>.Success(sale.ToResponse());
    }
}
