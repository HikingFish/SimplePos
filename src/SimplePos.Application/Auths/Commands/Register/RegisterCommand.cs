using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Auths.Commands.Register
{
    public record RegisterCommand(
        string Username,
        string Email,
        string Password,
        Guid OutletId,
        string PhoneNumber,
        string UserPosition);
}
