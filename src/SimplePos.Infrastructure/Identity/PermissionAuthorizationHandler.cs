using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SimplePos.Application.Abstractions.Identity;

namespace SimplePos.Infrastructure.Identity;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionCacheService _permissionCache;

    public PermissionAuthorizationHandler(IPermissionCacheService permissionCache)
    {
        _permissionCache = permissionCache;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub) ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

        if(userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return;
        }

        var permissions = await _permissionCache.GetPermissionsAsync(userId);
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}