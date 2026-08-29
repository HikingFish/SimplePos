using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Auths.Queries;
public record GetCurrentUserQuery(Guid UserId) : IQuery<Result<CurrentUserResponse>>;
