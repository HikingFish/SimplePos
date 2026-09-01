using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Users.Commands.GrantOutletAccess;

public record GrantUserOutletAccessCommand(Guid UserId, Guid OutletId) : ICommand<Result>;
