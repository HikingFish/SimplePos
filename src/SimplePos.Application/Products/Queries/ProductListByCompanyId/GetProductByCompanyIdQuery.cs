using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Products.Queries.ProductListByCompanyId;

public record GetProductByCompanyIdQuery(Guid CompanyId, int Page, int PageSize) : IQuery<Result<PagedList<ProductResponse>>>;