using System;

namespace SimplePos.Application.Categories.Common;

public record CategoryResponse(
    Guid CategoryId,
    string Name,
    bool IsActive
);
