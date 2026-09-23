using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Taxes.Commands.UpdateTax;

public record UpdateTaxCommand(Guid TaxId, Guid CompanyId, string TaxName, decimal TaxRate) : ICommand<Result>;
