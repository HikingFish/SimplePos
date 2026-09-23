using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.PaymentMethods.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Payments;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.PaymentMethods.Queries.GetActivePaymentMethodsByCompany;

public class GetActivePaymentMethodsByCompanyQueryHandler : IQueryHandler<GetActivePaymentMethodsByCompanyQuery, Result<List<PaymentMethodResponse>>>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetActivePaymentMethodsByCompanyQueryHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<Result<List<PaymentMethodResponse>>> HandleAsync(GetActivePaymentMethodsByCompanyQuery query, CancellationToken cancellationToken)
    {
        List<PaymentMethod> paymentMethods = await _paymentMethodRepository.GetActivePaymentMethodsByCompanyIdAsync(query.CompanyId);

        List<PaymentMethodResponse> responses = paymentMethods
            .Where(pm => !pm.SoftDeleted && pm.IsActive)
            .Select(pm => new PaymentMethodResponse(pm.PaymentMethodId, pm.CompanyId, pm.Name, pm.IsActive))
            .ToList();

        return Result<List<PaymentMethodResponse>>.Success(responses);
    }
}
