using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Taxes.Commands.ActivateTax;

public class ActivateTaxCommandHandler : ICommandHandler<ActivateTaxCommand, Result>
{
    private readonly ITaxRepository _taxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateTaxCommandHandler(ITaxRepository taxRepository, IUnitOfWork unitOfWork)
    {
        _taxRepository = taxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ActivateTaxCommand command, CancellationToken cancellationToken)
    {
        Tax? tax = await _taxRepository.GetTaxByIdAsync(command.TaxId);
        if (tax == null || tax.CompanyId != command.CompanyId)
        {
            return Result.Failure(TaxError.NotExist);
        }

        Result result = tax.Activate();
        if (!result.IsSuccess)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
