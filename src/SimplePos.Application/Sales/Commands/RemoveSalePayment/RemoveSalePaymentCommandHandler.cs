using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.RemoveSalePayment;

public class RemoveSalePaymentCommandHandler : ICommandHandler<RemoveSalePaymentCommand, Result>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveSalePaymentCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RemoveSalePaymentCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result.Failure(SaleError.SaleNotFound);
        }

        SalePayment? payment = sale.SalePayments.FirstOrDefault(s => s.SalePaymentId == command.PaymentId);
        Result result = sale.RemovePayment(command.PaymentId);
        if (!result.IsSuccess)
        {
            return result;
        }

        if (payment != null)
        {
            _saleRepository.RemovePayment(payment);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
