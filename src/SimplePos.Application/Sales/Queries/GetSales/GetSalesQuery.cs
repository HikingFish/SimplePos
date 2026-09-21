using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;

namespace SimplePos.Application.Sales.Queries.GetSales;

public record GetSalesQuery(
    Guid? OutletId,
    DateTime? From,
    DateTime? To,
    Guid? UserId
) : IQuery<Result<List<SaleResponse>>>;
