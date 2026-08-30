using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Users.Queries.GetOutletAccess;

public record GetUserOutletAccessesQuery(Guid UserId) : IQuery<Result<List<UserOutletAccessResponse>>>;
