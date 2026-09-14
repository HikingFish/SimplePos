using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IProductRepository repository, IUnitOfWork unitOfWork)
    {
        _productRepository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        Product? productResult = await _productRepository.GetProductByIdAsync(command.ProductId);

        if (productResult == null || productResult.CompanyId != command.CompanyId)
        {
            return Result.Failure(ProductError.ProductNotExist);
        }

        Result softDeleteResult = productResult.SoftDelete();
        if (!softDeleteResult.IsSuccess)
        {
            return softDeleteResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}