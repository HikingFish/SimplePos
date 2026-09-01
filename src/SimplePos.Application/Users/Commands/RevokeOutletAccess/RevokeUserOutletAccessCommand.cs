using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Users.Commands.RevokeOutletAccess;

public record RevokeUserOutletAccessCommand(Guid UserId, Guid OutletId) : ICommand<Result>;
