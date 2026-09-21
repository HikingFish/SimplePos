using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.VoidSale;

public class VoidSaleCommandHandler : ICommandHandler<VoidSaleCommand, Result>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VoidSaleCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(VoidSaleCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result.Failure(SaleError.SaleNotFound);
        }

        Result result = sale.VoidSale(command.UserId);
        if (!result.IsSuccess)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
