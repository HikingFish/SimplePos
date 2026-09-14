using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Categories.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;

namespace SimplePos.Application.Categories.Queries.GetCategoriesByCompany;

public record GetCategoriesByCompanyQuery(Guid CompanyId) : IQuery<Result<List<CategoryResponse>>>;
