using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Taxes.Commands.UpdateTax;

public class UpdateTaxCommandHandler : ICommandHandler<UpdateTaxCommand, Result>
{
    private readonly ITaxRepository _taxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaxCommandHandler(ITaxRepository taxRepository, IUnitOfWork unitOfWork)
    {
        _taxRepository = taxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateTaxCommand command, CancellationToken cancellationToken)
    {
        Tax? tax = await _taxRepository.GetTaxByIdAsync(command.TaxId);
        if (tax == null || tax.CompanyId != command.CompanyId || tax.SoftDeleted)
        {
            return Result.Failure(TaxError.NotExist);
        }

        Result updateResult = tax.UpdateTaxInfo(command.TaxName, command.TaxRate);
        if (!updateResult.IsSuccess)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
