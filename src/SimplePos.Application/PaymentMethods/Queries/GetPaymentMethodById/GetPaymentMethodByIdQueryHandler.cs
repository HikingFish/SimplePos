using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.PaymentMethods.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Payments;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.PaymentMethods.Queries.GetPaymentMethodById;

public class GetPaymentMethodByIdQueryHandler : IQueryHandler<GetPaymentMethodByIdQuery, Result<PaymentMethodResponse>>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetPaymentMethodByIdQueryHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<Result<PaymentMethodResponse>> HandleAsync(GetPaymentMethodByIdQuery query, CancellationToken cancellationToken)
    {
        PaymentMethod? paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(query.PaymentMethodId);
        if (paymentMethod == null || paymentMethod.CompanyId != query.CompanyId || paymentMethod.SoftDeleted)
        {
            return Result<PaymentMethodResponse>.Failure(PaymentMethodError.NotExist);
        }

        var response = new PaymentMethodResponse(paymentMethod.PaymentMethodId, paymentMethod.CompanyId, paymentMethod.Name, paymentMethod.IsActive);
        return Result<PaymentMethodResponse>.Success(response);
    }
}
