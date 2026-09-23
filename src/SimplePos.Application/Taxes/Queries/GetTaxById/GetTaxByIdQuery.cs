using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Taxes.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Taxes.Queries.GetTaxById;

public record GetTaxByIdQuery(Guid TaxId, Guid CompanyId) : IQuery<Result<TaxResponse>>;
