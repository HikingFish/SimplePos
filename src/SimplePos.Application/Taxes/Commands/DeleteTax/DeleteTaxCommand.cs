using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Taxes.Commands.DeleteTax;

public record DeleteTaxCommand(Guid TaxId, Guid CompanyId) : ICommand<Result>;
