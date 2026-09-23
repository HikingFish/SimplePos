using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Application.Products.Commands.AddProductTax;

public class AddProductTaxCommandHandler : ICommandHandler<AddProductTaxCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProductTaxCommandHandler(
        IProductRepository productRepository,
        ITaxRepository taxRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _taxRepository = taxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(AddProductTaxCommand command, CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetProductByIdAsync(command.ProductId);
        if (product == null || product.CompanyId != command.CompanyId || product.SoftDeleted)
        {
            return Result.Failure(ProductError.ProductNotExist);
        }

        Tax? tax = await _taxRepository.GetTaxByIdAsync(command.TaxId);
        if (tax == null || tax.CompanyId != command.CompanyId || tax.SoftDeleted)
        {
            return Result.Failure(TaxError.NotExist);
        }

        Result addResult = product.AddProductTax(tax.TaxId);
        if (!addResult.IsSuccess)
        {
            return addResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
