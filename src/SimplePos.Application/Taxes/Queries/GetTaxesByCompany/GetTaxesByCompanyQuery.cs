using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Taxes.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;

namespace SimplePos.Application.Taxes.Queries.GetTaxesByCompany;

public record GetTaxesByCompanyQuery(Guid CompanyId) : IQuery<Result<List<TaxResponse>>>;
