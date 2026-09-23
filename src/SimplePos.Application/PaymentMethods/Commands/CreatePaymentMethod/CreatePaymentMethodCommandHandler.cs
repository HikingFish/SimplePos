using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Payments;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.PaymentMethods.Commands.CreatePaymentMethod;

public class CreatePaymentMethodCommandHandler : ICommandHandler<CreatePaymentMethodCommand, Result<Guid>>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreatePaymentMethodCommand command, CancellationToken cancellationToken)
    {
        Result<PaymentMethod> paymentMethodResult = PaymentMethod.Create(command.CompanyId, command.Name);
        if (!paymentMethodResult.IsSuccess)
        {
            return Result<Guid>.Failure(paymentMethodResult.Error);
        }

        _paymentMethodRepository.AddPaymentMethod(paymentMethodResult.Data!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(paymentMethodResult.Data!.PaymentMethodId);
    }
}
