using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Auths.Queries;

public record CurrentUserResponse(
    string UserName,
    string Email,
    string? PhoneNumber,
    string? UserPosition,
    bool IsActive,
    CurrentCompany Company,
    CurrentOutlet Outlet,
    IReadOnlyCollection<string> Permissions
);

public record CurrentCompany(
    string Name,
    string Email,
    string? PhoneNumber
);

public record CurrentOutlet(
    string Name,
    string? PhoneNumber
);
