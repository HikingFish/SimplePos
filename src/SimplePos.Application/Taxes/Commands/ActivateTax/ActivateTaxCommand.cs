using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Taxes.Commands.ActivateTax;

public record ActivateTaxCommand(Guid TaxId, Guid CompanyId) : ICommand<Result>;
