using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Companies.Queries
{
    public record GetCompanyByIdQuery(Guid companyId) : IQuery<Result<CompanyResponse>>;
}
