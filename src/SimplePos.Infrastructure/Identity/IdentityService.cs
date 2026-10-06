using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Application.Auths.Common;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Users;
using SimplePos.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimplePos.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IPermissionCacheService _permissionCacheService;
    private readonly AppDbContext _context;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenProvider tokenProvider,
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IUserPermissionRepository userPermissionRepository,
        IPermissionCacheService permissionCacheService,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenProvider;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _userPermissionRepository = userPermissionRepository;
        _permissionCacheService = permissionCacheService;
        _context = context;
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            return Result<AuthResponse>.Failure(UserError.InvalidCredentials);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(appUser, password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Result<AuthResponse>.Failure(UserError.InvalidCredentials);
        }

        var domainUser = await _userRepository.GetUserByIdAsync(appUser.DomainUserId);
        if (domainUser == null)
        {
            return Result<AuthResponse>.Failure(new Error("Identity.UserNotFound", "User domain entity not found", ErrorType.NotFound));
        }

        var userCompany = await _companyRepository.GetCompanyByIdAsync(domainUser.CompanyId);
        if (userCompany is null)
        {
            return Result<AuthResponse>.Failure(IdentityError.CompanyNotFound);
        }

        var accessToken = _tokenProvider.CreateToken(domainUser);

        var userPermissionsResult = await _userPermissionRepository.GetPermissionsByUserIdAsync(domainUser.UserId);
        var userPermissions = userPermissionsResult.Select(p => p.Name).ToHashSet();

        await _permissionCacheService.SetPermissionAsync(domainUser.UserId, userPermissions);

        var refreshTokenString = _tokenProvider.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            ApplicationUserId = appUser.Id,
            Token = refreshTokenString,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshTokenString, refreshToken.ExpiresAtUtc));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<AuthResponse>.Failure(new Error("Auth.RefreshTokenEmpty", "Refresh token cannot be empty", ErrorType.Validation));
        }

        var existingToken = await _context.RefreshTokens
            .Include(r => r.ApplicationUser)
            .FirstOrDefaultAsync(r => r.Token == refreshToken, ct);

        if (existingToken is null)
        {
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidToken", "Invalid refresh token", ErrorType.Unauthorized));
        }

        // Reuse detection: If a revoked token is used, assume breach and revoke all active tokens for this user
        if (existingToken.IsRevoked)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(r => r.ApplicationUserId == existingToken.ApplicationUserId && r.RevokedAtUtc == null)
                .ToListAsync(ct);

            foreach (var token in activeTokens)
            {
                token.RevokedAtUtc = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);

            return Result<AuthResponse>.Failure(new Error("Auth.TokenCompromised", "Revoked token reuse detected. All sessions invalidated.", ErrorType.Unauthorized));
        }

        if (existingToken.IsExpired)
        {
            return Result<AuthResponse>.Failure(new Error("Auth.TokenExpired", "Refresh token has expired", ErrorType.Unauthorized));
        }

        var domainUser = await _userRepository.GetUserByIdAsync(existingToken.ApplicationUser.DomainUserId);
        if (domainUser is null || !domainUser.IsActive || domainUser.SoftDeleted)
        {
            return Result<AuthResponse>.Failure(UserError.SoftDeleted);
        }

        // Rotate: revoke current and issue new
        var newRefreshTokenString = _tokenProvider.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            ApplicationUserId = existingToken.ApplicationUserId,
            Token = newRefreshTokenString,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        existingToken.RevokedAtUtc = DateTime.UtcNow;
        existingToken.ReplacedByToken = newRefreshTokenString;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(ct);

        var newAccessToken = _tokenProvider.CreateToken(domainUser);

        return Result<AuthResponse>.Success(new AuthResponse(newAccessToken, newRefreshTokenString, newRefreshToken.ExpiresAtUtc));
    }

    public async Task<Result> RevokeTokenAsync(string refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Failure(new Error("Auth.RefreshTokenEmpty", "Refresh token cannot be empty", ErrorType.Validation));
        }

        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken, ct);

        if (existingToken is null)
        {
            return Result.Failure(new Error("Auth.TokenNotFound", "Refresh token not found", ErrorType.NotFound));
        }

        if (existingToken.IsRevoked)
        {
            return Result.Success();
        }

        existingToken.RevokedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result<Guid>> RegisterUserAsync(Guid domainUserId, string username, string email, string password)
    {
        ApplicationUser appUser = new ApplicationUser
        {
            DomainUserId = domainUserId,
            Email = email, 
            UserName = username
        };

        var identityResult = await _userManager.CreateAsync(appUser, password);

        if (!identityResult.Succeeded)
        {
            var errorMessage = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return Result<Guid>.Failure(new Error("Identity.RegistrationFailed", errorMessage, ErrorType.Validation));
        }

        return Result<Guid>.Success(appUser.DomainUserId);
    }
}
