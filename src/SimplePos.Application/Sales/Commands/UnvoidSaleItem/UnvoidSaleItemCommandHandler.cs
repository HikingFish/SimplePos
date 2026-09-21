using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.UnvoidSaleItem;

public class UnvoidSaleItemCommandHandler : ICommandHandler<UnvoidSaleItemCommand, Result>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnvoidSaleItemCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UnvoidSaleItemCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result.Failure(SaleError.SaleNotFound);
        }

        Result result = sale.UnvoidSaleItem(command.SaleItemId);
        if (!result.IsSuccess)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
