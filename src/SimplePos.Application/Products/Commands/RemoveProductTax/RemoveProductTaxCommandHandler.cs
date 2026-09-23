using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Products.Commands.RemoveProductTax;

public class RemoveProductTaxCommandHandler : ICommandHandler<RemoveProductTaxCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProductTaxCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RemoveProductTaxCommand command, CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetProductByIdAsync(command.ProductId);
        if (product == null || product.CompanyId != command.CompanyId || product.SoftDeleted)
        {
            return Result.Failure(ProductError.ProductNotExist);
        }

        Result removeResult = product.RemoveProductTax(command.TaxId);
        if (!removeResult.IsSuccess)
        {
            return removeResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
