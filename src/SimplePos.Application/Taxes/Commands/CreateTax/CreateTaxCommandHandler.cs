using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Taxes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Taxes.Commands.CreateTax;

public class CreateTaxCommandHandler : ICommandHandler<CreateTaxCommand, Result<Guid>>
{
    private readonly ITaxRepository _taxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaxCommandHandler(ITaxRepository taxRepository, IUnitOfWork unitOfWork)
    {
        _taxRepository = taxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateTaxCommand command, CancellationToken cancellationToken)
    {
        Result<Tax> taxResult = Tax.Create(command.CompanyId, command.TaxName, command.TaxRate);
        if (!taxResult.IsSuccess)
        {
            return Result<Guid>.Failure(taxResult.Error);
        }

        _taxRepository.AddTax(taxResult.Data!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(taxResult.Data!.TaxId);
    }
}
