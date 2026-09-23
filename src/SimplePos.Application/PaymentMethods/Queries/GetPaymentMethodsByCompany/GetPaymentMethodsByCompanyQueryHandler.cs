using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.PaymentMethods.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Payments;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.PaymentMethods.Queries.GetPaymentMethodsByCompany;

public class GetPaymentMethodsByCompanyQueryHandler : IQueryHandler<GetPaymentMethodsByCompanyQuery, Result<List<PaymentMethodResponse>>>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetPaymentMethodsByCompanyQueryHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<Result<List<PaymentMethodResponse>>> HandleAsync(GetPaymentMethodsByCompanyQuery query, CancellationToken cancellationToken)
    {
        List<PaymentMethod> paymentMethods = await _paymentMethodRepository.GetPaymentMethodsByCompanyIdAsync(query.CompanyId);

        List<PaymentMethodResponse> responses = paymentMethods
            .Where(pm => !pm.SoftDeleted)
            .Select(pm => new PaymentMethodResponse(pm.PaymentMethodId, pm.CompanyId, pm.Name, pm.IsActive))
            .ToList();

        return Result<List<PaymentMethodResponse>>.Success(responses);
    }
}
