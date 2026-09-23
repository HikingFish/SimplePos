using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Taxes.Commands.DeactivateTax;

public record DeactivateTaxCommand(Guid TaxId, Guid CompanyId) : ICommand<Result>;
