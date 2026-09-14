using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Products.Queries.ProductByProductId;
public record GetProductByProductIdQuery(Guid ProductId, Guid CompanyId) : IQuery<Result<ProductResponse>>;