using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Taxes.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Taxes.Queries.GetTaxById;

public class GetTaxByIdQueryHandler : IQueryHandler<GetTaxByIdQuery, Result<TaxResponse>>
{
    private readonly ITaxRepository _taxRepository;

    public GetTaxByIdQueryHandler(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository;
    }

    public async Task<Result<TaxResponse>> HandleAsync(GetTaxByIdQuery query, CancellationToken cancellationToken)
    {
        Tax? tax = await _taxRepository.GetTaxByIdAsync(query.TaxId);
        if (tax == null || tax.CompanyId != query.CompanyId || tax.SoftDeleted)
        {
            return Result<TaxResponse>.Failure(TaxError.NotExist);
        }

        var response = new TaxResponse(tax.TaxId, tax.CompanyId, tax.TaxName, tax.TaxRate, tax.IsActive);
        return Result<TaxResponse>.Success(response);
    }
}
