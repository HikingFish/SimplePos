using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Users.Commands.SetOutletAccess;

public record SetUserOutletAccessesCommand(Guid UserId, List<Guid> OutletIds) : ICommand<Result>;
