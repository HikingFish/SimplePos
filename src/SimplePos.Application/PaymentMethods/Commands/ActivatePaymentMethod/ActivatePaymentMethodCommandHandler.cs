using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Payments;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.PaymentMethods.Commands.ActivatePaymentMethod;

public class ActivatePaymentMethodCommandHandler : ICommandHandler<ActivatePaymentMethodCommand, Result>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivatePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ActivatePaymentMethodCommand command, CancellationToken cancellationToken)
    {
        PaymentMethod? paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(command.PaymentMethodId);
        if (paymentMethod == null || paymentMethod.CompanyId != command.CompanyId)
        {
            return Result.Failure(PaymentMethodError.NotExist);
        }

        Result result = paymentMethod.UpdateToActiveStatus();
        if (!result.IsSuccess)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
