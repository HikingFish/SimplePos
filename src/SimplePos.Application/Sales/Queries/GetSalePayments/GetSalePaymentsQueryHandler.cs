using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Queries.GetSalePayments;

public class GetSalePaymentsQueryHandler : IQueryHandler<GetSalePaymentsQuery, Result<List<SalePaymentResponse>>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalePaymentsQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<Result<List<SalePaymentResponse>>> HandleAsync(GetSalePaymentsQuery query, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(query.SaleId);
        if (sale == null)
        {
            return Result<List<SalePaymentResponse>>.Failure(SaleError.SaleNotFound);
        }

        var payments = sale.SalePayments.Select(p => p.ToResponse()).ToList();
        return Result<List<SalePaymentResponse>>.Success(payments);
    }
}
