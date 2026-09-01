using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Sales.Commands.CreateSale;

public class CreateCommandSaleCommandHandler : ICommandHandler<CreateSaleCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOutletProductAvailabilityRepository _outletProductAvailability;
    private readonly IProductRepository _productRepository;
    private readonly IUserOutletAccessRepository _userOutletAccessRepository;
    private readonly ISaleRepository _saleRepository;

    public CreateCommandSaleCommandHandler(IUserRepository userRepository, IOutletProductAvailabilityRepository outletProductAvailability, IProductRepository productRepository, IUserOutletAccessRepository userOutletAccessRepository, ISaleRepository saleRepository)
    {
        _userRepository = userRepository;
        _outletProductAvailability = outletProductAvailability;
        _productRepository = productRepository;
        _userOutletAccessRepository = userOutletAccessRepository;
        _saleRepository = saleRepository;
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

        Result<Sale> newSale = Sale.Create(saleOutletId, command.UserId, null, command.financialDate, null);
        if (newSale.IsFailure)
            return Result<Guid>.Failure(newSale.Error);

        foreach (CreateSaleItem saleItem in command.SaleItems)
        {
            Product? product = await _productRepository.GetProductByIdAsync(saleItem.productId);
            if(product == null)
                return Result<Guid>.Failure(SaleError.SaleItemNotFound);

            OutletProductAvailability? outletAvailability = await _outletProductAvailability.GetByOutletAndProductIdAsync(saleOutletId, product.ProductId);

            if(outletAvailability == null)
                return Result<Guid>.Failure(OutletProductAvailabilityError.AlreadyUnavailable);

            SaleItem newSaleItem = SaleItem.Create(saleItem.productId, newSale.Data.SaleId, command.UserId, saleItem.quantity, )
        }
    }
}