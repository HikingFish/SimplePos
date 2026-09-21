using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.VoidSaleItem;

public class VoidSaleItemCommandHandler : ICommandHandler<VoidSaleItemCommand, Result>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VoidSaleItemCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(VoidSaleItemCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result.Failure(SaleError.SaleNotFound);
        }

        Result result = sale.VoidSaleItem(command.SaleItemId, command.UserId);
        if (!result.IsSuccess)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
