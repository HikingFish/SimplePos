using Microsoft.AspNetCore.Identity;
using Npgsql.Internal;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Users;
using SimplePos.Infrastructure.Persistence.Permissions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimplePos.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPermissionCacheService _permissionCacheService;
    private readonly IPermissionRepository _permissionRepository;

    public IdentityService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenProvider tokenProvider,
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IPermissionCacheService permissionCacheService,
        IPermissionRepository permissionRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenProvider;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _permissionCacheService = permissionCacheService;
    }

    public async Task<Result<string>> LoginAsync(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if(appUser == null)
        {
            return Result<string>.Failure(UserError.InvalidCredentials);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(appUser, password, lockoutOnFailure: false);
        if(!result.Succeeded)
        {
            return Result<string>.Failure(UserError.InvalidCredentials);

        }

        var domainUser = await _userRepository.GetUserByIdAsync(appUser.DomainUserId);
        if (domainUser == null)
        {
            return Result<string>.Failure(new Error("Identity.UserNotFound", "User domain entity not found", ErrorType.NotFound));
        }

        var userCompany = await _companyRepository.GetCompanyByIdAsync(domainUser.CompanyId);

        if(userCompany is null)
        {
            return Result<string>.Failure(IdentityError.CompanyNotFound);
        }

        var token = _tokenProvider.CreateToken(domainUser);
        var permissionsResult = await _permissionRepository.GetPermissionsByUserIdAsync(domainUser.UserId);
        var permissions = permissionsResult.Select(p => p.Name).ToList();
        await _permissionCacheService.SetPermissionAsync(domainUser.UserId, permissions);

        return Result<string>.Success(token);
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
