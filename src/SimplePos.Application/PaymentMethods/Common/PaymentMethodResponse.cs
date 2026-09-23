using System;

namespace SimplePos.Application.PaymentMethods.Common;

public record PaymentMethodResponse(
    Guid PaymentMethodId,
    Guid CompanyId,
    string Name,
    bool IsActive
);
