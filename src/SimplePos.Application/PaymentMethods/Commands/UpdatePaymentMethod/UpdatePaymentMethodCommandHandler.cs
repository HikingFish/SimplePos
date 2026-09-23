using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Payments;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.PaymentMethods.Commands.UpdatePaymentMethod;

public class UpdatePaymentMethodCommandHandler : ICommandHandler<UpdatePaymentMethodCommand, Result>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdatePaymentMethodCommand command, CancellationToken cancellationToken)
    {
        PaymentMethod? paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(command.PaymentMethodId);
        if (paymentMethod == null || paymentMethod.CompanyId != command.CompanyId || paymentMethod.SoftDeleted)
        {
            return Result.Failure(PaymentMethodError.NotExist);
        }

        Result updateResult = paymentMethod.UpdateName(command.Name);
        if (!updateResult.IsSuccess)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
