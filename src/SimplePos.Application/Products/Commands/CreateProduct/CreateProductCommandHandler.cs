using SimplePos.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    public Task HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}