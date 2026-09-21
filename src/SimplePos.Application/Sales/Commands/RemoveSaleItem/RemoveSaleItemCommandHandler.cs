using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.RemoveSaleItem;

public class RemoveSaleItemCommandHandler : ICommandHandler<RemoveSaleItemCommand, Result>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveSaleItemCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RemoveSaleItemCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result.Failure(SaleError.SaleNotFound);
        }

        SaleItem? saleItem = sale.SaleItems.FirstOrDefault(s => s.SaleItemId == command.SaleItemId);
        Result result = sale.RemoveSaleItem(command.SaleItemId);
        if (!result.IsSuccess)
        {
            return result;
        }

        if (saleItem != null)
        {
            _saleRepository.RemoveSaleItem(saleItem);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
