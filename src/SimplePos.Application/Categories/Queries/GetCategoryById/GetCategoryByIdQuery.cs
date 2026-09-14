using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Categories.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid CategoryId, Guid CompanyId) : IQuery<Result<CategoryResponse>>;
