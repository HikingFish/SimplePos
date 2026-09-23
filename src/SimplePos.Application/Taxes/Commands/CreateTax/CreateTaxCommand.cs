using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Taxes.Commands.CreateTax;

public record CreateTaxCommand(Guid CompanyId, string TaxName, decimal TaxRate) : ICommand<Result<Guid>>;
