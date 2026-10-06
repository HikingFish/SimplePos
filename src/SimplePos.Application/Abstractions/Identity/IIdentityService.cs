using SimplePos.Application.Auths.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Abstractions.Identity;
public interface IIdentityService
{
    Task<Result<Guid>> RegisterUserAsync(Guid domainUserId, string username, string email, string password);
    Task<Result<AuthResponse>> LoginAsync(string email, string password);
    Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken ct);
    Task<Result> RevokeTokenAsync(string refreshToken, CancellationToken ct);
}