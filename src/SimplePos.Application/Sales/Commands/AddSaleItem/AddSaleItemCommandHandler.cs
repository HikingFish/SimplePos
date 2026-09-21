using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Sales.Commands.AddSaleItem;

public class AddSaleItemCommandHandler : ICommandHandler<AddSaleItemCommand, Result<Guid>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOutletProductAvailabilityRepository _outletProductAvailabilityRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddSaleItemCommandHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IOutletProductAvailabilityRepository outletProductAvailabilityRepository,
        ITaxRepository taxRepository,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _outletProductAvailabilityRepository = outletProductAvailabilityRepository;
        _taxRepository = taxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(AddSaleItemCommand command, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetSaleByIdAsync(command.SaleId);
        if (sale == null)
        {
            return Result<Guid>.Failure(SaleError.SaleNotFound);
        }

        Product? product = await _productRepository.GetProductByIdAsync(command.ProductId);
        if (product == null)
        {
            return Result<Guid>.Failure(SaleError.SaleItemNotFound);
        }

        OutletProductAvailability? outletAvailability = await _outletProductAvailabilityRepository.GetByOutletAndProductIdAsync(sale.OutletId, product.ProductId);
        if (outletAvailability != null)
        {
            return Result<Guid>.Failure(OutletProductAvailabilityError.ProductIdEmpty);
        }

        decimal price = command.Price ?? product.BasePrice;
        decimal discount = command.Discount ?? 0;

        List<Tax> taxes = await _taxRepository.GetActiveTaxesByCompanyIdAsync(command.CompanyId);
        Dictionary<Guid, Tax> taxDictionary = taxes.ToDictionary(t => t.TaxId, t => t);

        List<Guid> taxIds = product.ProductTaxes.Select(pt => pt.TaxId).ToList();
        List<Tax> applicableTaxes = new List<Tax>();
        foreach (Guid taxId in taxIds)
        {
            Tax? tax = taxDictionary.GetValueOrDefault(taxId);
            if (tax == null)
            {
                return Result<Guid>.Failure(TaxError.TaxNameEmpty);
            }
            applicableTaxes.Add(tax);
        }

        Result<SaleItem> newSaleItemResult = SaleItem.Create(
            command.ProductId,
            sale.SaleId,
            command.UserId,
            command.Quantity,
            price,
            discount,
            command.Remarks,
            applicableTaxes,
            null,
            command.FinancialDate);

        if (newSaleItemResult.IsFailure)
        {
            return Result<Guid>.Failure(newSaleItemResult.Error);
        }

        Result addSaleItemResult = sale.AddSaleItem(newSaleItemResult.Data!);
        if (addSaleItemResult.IsFailure)
        {
            return Result<Guid>.Failure(addSaleItemResult.Error);
        }

        _saleRepository.AddSaleItem(newSaleItemResult.Data!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(newSaleItemResult.Data!.SaleItemId);
    }
}
