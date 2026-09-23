using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Products.Commands.AddProductTax;

public record AddProductTaxCommand(Guid ProductId, Guid CompanyId, Guid TaxId) : ICommand<Result>;
