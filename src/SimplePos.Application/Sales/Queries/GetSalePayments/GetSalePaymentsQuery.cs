using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Sales.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;

namespace SimplePos.Application.Sales.Queries.GetSalePayments;

public record GetSalePaymentsQuery(Guid SaleId) : IQuery<Result<List<SalePaymentResponse>>>;
