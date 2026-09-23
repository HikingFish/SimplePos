using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.PaymentMethods.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;

namespace SimplePos.Application.PaymentMethods.Queries.GetActivePaymentMethodsByCompany;

public record GetActivePaymentMethodsByCompanyQuery(Guid CompanyId) : IQuery<Result<List<PaymentMethodResponse>>>;
