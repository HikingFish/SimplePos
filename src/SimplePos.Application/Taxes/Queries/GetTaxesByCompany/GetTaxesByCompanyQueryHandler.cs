using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Taxes.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Taxes.Queries.GetTaxesByCompany;

public class GetTaxesByCompanyQueryHandler : IQueryHandler<GetTaxesByCompanyQuery, Result<List<TaxResponse>>>
{
    private readonly ITaxRepository _taxRepository;

    public GetTaxesByCompanyQueryHandler(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository;
    }

    public async Task<Result<List<TaxResponse>>> HandleAsync(GetTaxesByCompanyQuery query, CancellationToken cancellationToken)
    {
        List<Tax> taxes = await _taxRepository.GetTaxesByCompanyIdAsync(query.CompanyId);

        List<TaxResponse> responses = taxes
            .Where(t => !t.SoftDeleted)
            .Select(t => new TaxResponse(t.TaxId, t.CompanyId, t.TaxName, t.TaxRate, t.IsActive))
            .ToList();

        return Result<List<TaxResponse>>.Success(responses);
    }
}
