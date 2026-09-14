using System;

namespace SimplePos.Application.Categories.Common;

public record CategoryResponse(
    Guid CategoryId,
    Guid CompanyId,
    string Name,
    bool IsActive
);
