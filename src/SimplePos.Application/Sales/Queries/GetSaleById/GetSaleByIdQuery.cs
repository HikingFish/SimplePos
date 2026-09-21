using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Sales.Queries.GetSaleById;

public record GetSaleByIdQuery(Guid SaleId) : IQuery<Result<SaleResponse>>;
