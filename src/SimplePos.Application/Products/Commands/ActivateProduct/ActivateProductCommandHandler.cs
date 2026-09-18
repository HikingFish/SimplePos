using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.ActivateProduct;

public class ActivateProductCommandHandler : ICommandHandler<ActivateProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ActivateProductCommand command, CancellationToken cancellationToken)
    {
        Product? productResult = await _productRepository.GetProductByIdAsync(command.ProductId);
        if (productResult == null)
            return Result.Failure(ProductError.ProductNotExist);

        if (productResult.CompanyId != command.CompanyId)
            return Result.Failure(ProductError.ProductNotExist);

        productResult.UpdateToNotActiveStatus();

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
