using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Sales.Commands.CreateSale;

public class CreateCommandSaleCommandHandler : ICommandHandler<CreateSaleCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOutletProductAvailabilityRepository _outletProductAvailability;
    private readonly IProductRepository _productRepository;
    private readonly IUserOutletAccessRepository _userOutletAccessRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommandSaleCommandHandler(IUserRepository userRepository, IOutletProductAvailabilityRepository outletProductAvailability, IProductRepository productRepository, IUserOutletAccessRepository userOutletAccessRepository, ISaleRepository saleRepository, ITaxRepository taxRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _outletProductAvailability = outletProductAvailability;
        _productRepository = productRepository;
        _userOutletAccessRepository = userOutletAccessRepository;
        _saleRepository = saleRepository;
        _taxRepository = taxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        Guid saleOutletId = new Guid();
        if(command.OutletId is Guid outletIdExist)
        {
            saleOutletId = outletIdExist;
        }
        else
        {
            User? currentUser = await _userRepository.GetUserByIdAsync(command.UserId);
            if(currentUser == null)
                return Result<Guid>.Failure(UserError.AccountNotFound);

            if(currentUser.OutletId == null)
                return Result<Guid>.Failure(OutletError.OutletNotFound);

            saleOutletId = currentUser.OutletId.Value;
        }

        Result<Sale> newSale = Sale.Create(saleOutletId, command.UserId, string.Empty, command.financialDate, null);
        if (newSale.IsFailure)
            return Result<Guid>.Failure(newSale.Error);

        if (newSale.Data == null)
            return Result<Guid>.Failure(SaleError.SaleItemNull);

        List<Tax> taxes = await _taxRepository.GetActiveTaxesByCompanyIdAsync(command.CompanyId); 
        Dictionary<Guid, Tax> taxDictionary = taxes.ToDictionary(t => t.TaxId, t => t);


        foreach (CreateSaleItem saleItem in command.SaleItems)
        {
            Product? product = await _productRepository.GetProductByIdAsync(saleItem.productId);
            if(product == null)
                return Result<Guid>.Failure(SaleError.SaleItemNotFound);

            OutletProductAvailability? outletAvailability = await _outletProductAvailability.GetByOutletAndProductIdAsync(saleOutletId, product.ProductId);

            if(outletAvailability != null)
                return Result<Guid>.Failure(OutletProductAvailabilityError.ProductIdEmpty);

            decimal price = saleItem.price ?? product.BasePrice;
            decimal discount = saleItem.discount ?? 0;
            List<Guid> taxIds = product.ProductTaxes.Select(pt => pt.TaxId).ToList();
            List<Tax> applicableTaxes = new List<Tax>();
            foreach(Guid taxId in taxIds)
            {
                Tax? tax = taxDictionary.GetValueOrDefault(taxId);
                //fix in future for the tax null
                if (tax == null)
                    return Result<Guid>.Failure(TaxError.TaxNameEmpty);
                applicableTaxes.Add(tax);
            }

            Result<SaleItem> newSaleItemResult = SaleItem.Create(
                saleItem.productId,
                newSale.Data.SaleId,
                command.UserId, 
                saleItem.quantity, 
                price, 
                discount, 
                saleItem.remarks, 
                applicableTaxes, 
                null,
                saleItem.financialDate);

            if (newSaleItemResult.IsFailure)
                return Result<Guid>.Failure(newSaleItemResult.Error);

            if (newSaleItemResult.Data == null)
                throw new InvalidOperationException("Sale item creation failed unexpectedly.");

            Result addSaleItemResult = newSale.Data.AddSaleItem(newSaleItemResult.Data);

            if (addSaleItemResult.IsFailure)
                return Result<Guid>.Failure(addSaleItemResult.Error);
        }
        _saleRepository.AddSale(newSale.Data);

        await _unitOfWork.SaveChangesAsync();
        return Result<Guid>.Success(newSale.Data.SaleId);
    }
}