using System;

namespace SimplePos.Application.Taxes.Common;

public record TaxResponse(
    Guid TaxId,
    Guid CompanyId,
    string TaxName,
    decimal TaxRate,
    bool IsActive
);
