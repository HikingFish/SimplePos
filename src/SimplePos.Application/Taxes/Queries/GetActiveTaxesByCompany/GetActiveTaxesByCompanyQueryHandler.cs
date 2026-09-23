using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Taxes.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Taxes.Queries.GetActiveTaxesByCompany;

public class GetActiveTaxesByCompanyQueryHandler : IQueryHandler<GetActiveTaxesByCompanyQuery, Result<List<TaxResponse>>>
{
    private readonly ITaxRepository _taxRepository;

    public GetActiveTaxesByCompanyQueryHandler(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository;
    }

    public async Task<Result<List<TaxResponse>>> HandleAsync(GetActiveTaxesByCompanyQuery query, CancellationToken cancellationToken)
    {
        List<Tax> taxes = await _taxRepository.GetActiveTaxesByCompanyIdAsync(query.CompanyId);

        List<TaxResponse> responses = taxes
            .Where(t => !t.SoftDeleted && t.IsActive)
            .Select(t => new TaxResponse(t.TaxId, t.CompanyId, t.TaxName, t.TaxRate, t.IsActive))
            .ToList();

        return Result<List<TaxResponse>>.Success(responses);
    }
}
