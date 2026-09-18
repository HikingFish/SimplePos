using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Commands.ActivateProduct;

public record ActivateProductCommand(Guid ProductId, Guid CompanyId) : ICommand<Result>;
