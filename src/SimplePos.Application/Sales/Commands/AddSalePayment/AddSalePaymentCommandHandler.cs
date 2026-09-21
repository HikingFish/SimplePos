using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.AddSalePayment;

public class AddSalePaymentCommandHandler : ICommandHandler<AddSalePaymentCommand, Result<Guid>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddSalePaymentCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(AddSalePaymentCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result<Guid>.Failure(SaleError.SaleNotFound);
        }

        Result<SalePayment> paymentResult = SalePayment.Create(
            command.SaleId,
            command.PaymentMethodId,
            command.ProcessedByUserId,
            command.AmountPaid,
            command.ReferenceNumber,
            command.PaymentDate);

        if (paymentResult.IsFailure)
        {
            return Result<Guid>.Failure(paymentResult.Error);
        }

        Result addPaymentResult = sale.AddPayment(paymentResult.Data!);
        if (!addPaymentResult.IsSuccess)
        {
            return Result<Guid>.Failure(addPaymentResult.Error);
        }

        _saleRepository.AddPayment(paymentResult.Data!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(paymentResult.Data!.SalePaymentId);
    }
}
