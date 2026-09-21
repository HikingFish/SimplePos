using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.UpdateSaleItem;

public class UpdateSaleItemCommandHandler : ICommandHandler<UpdateSaleItemCommand, Result>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSaleItemCommandHandler(
        ISaleRepository saleRepository,
        ITaxRepository taxRepository,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _taxRepository = taxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateSaleItemCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result.Failure(SaleError.SaleNotFound);
        }

        List<Tax>? taxes = null;
        if (command.TaxIds != null)
        {
            List<Tax> activeTaxes = await _taxRepository.GetActiveTaxesByCompanyIdAsync(command.CompanyId);
            var taxDictionary = activeTaxes.ToDictionary(t => t.TaxId, t => t);

            taxes = new List<Tax>();
            foreach (var taxId in command.TaxIds)
            {
                if (taxDictionary.TryGetValue(taxId, out var tax))
                {
                    taxes.Add(tax);
                }
                else
                {
                    return Result.Failure(TaxError.TaxNameEmpty);
                }
            }
        }

        Result result = sale.UpdateSaleItem(
            command.SaleItemId,
            command.Quantity,
            command.UnitPrice,
            command.UnitDiscount,
            command.Remark,
            taxes);

        if (!result.IsSuccess)
        {
            return result;
        }

        if (command.TaxIds != null)
        {
            SaleItem? updatedItem = sale.SaleItems.FirstOrDefault(s => s.SaleItemId == command.SaleItemId);
            if (updatedItem != null && updatedItem.SaleItemTaxes.Count > 0)
            {
                _saleRepository.AddSaleItemTaxes(updatedItem.SaleItemTaxes);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
