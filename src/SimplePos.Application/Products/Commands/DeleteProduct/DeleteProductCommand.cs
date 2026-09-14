using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.DeleteProduct;
public record DeleteProductCommand(Guid ProductId, Guid CompanyId): ICommand<Result>;