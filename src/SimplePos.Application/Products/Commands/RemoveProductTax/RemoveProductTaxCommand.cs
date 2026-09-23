using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Products.Commands.RemoveProductTax;

public record RemoveProductTaxCommand(Guid ProductId, Guid CompanyId, Guid TaxId) : ICommand<Result>;
