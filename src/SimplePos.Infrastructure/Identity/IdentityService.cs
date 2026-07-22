using Microsoft.AspNetCore.Identity;
using Npgsql.Internal;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _userRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IOutletRepository _outletRepository;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenProvider tokenProvider, IUserRepository userRepository, IPermissionRepository permissionRepository, ICompanyRepository companyRepository, IOutletRepository outletRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenProvider;
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
        _companyRepository = companyRepository;
        _outletRepository = outletRepository;
    }

    public async Task<Result<string>> LoginAsync(string email, string password)
    {
        var appUser = await _userManager.FindByNameAsync(email);
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
            return Result<string>.Failure(new Error("Identity.UserNotFound", "User domain entity not found"));
        }

        var userOutlet = await _outletRepository.GetOutletByIdAsync(domainUser.OutletId);

        var userCompany = await _companyRepository.GetCompanyByIdAsync(userOutlet.CompanyId);

        var userPermissionsId = domainUser.UserPermissions.Select(up => up.PermissionId);

        var permissions = await _permissionRepository.GetAllPermissionsAsync();

        var userPermissions = permissions
            .Where(p => userPermissionsId
            .Contains(p.PermissionId))
            .Select(p => p.Name);

        var token = _tokenProvider.CreateToken(domainUser, userCompany, userPermissions);
        return Result<string>.Success(token);
    }

    public async Task<Result<Guid>> RegisterUserAsync(string username, string email, string password, Guid outletId, string phoneNumber, string userPosition)
    {
        var newUserEmail = EmailAddress.Create(email);

        if(newUserEmail.IsFailure)
            return Result<Guid>.Failure(newUserEmail.Error);

        var newUser = User.Create(outletId, username, newUserEmail.Data, phoneNumber, userPosition);

        ApplicationUser appUser = new ApplicationUser{
            DomainUserId = newUser.Data.UserId,
            Email = newUser.Data.Email.ToString(), 
            UserName = newUser.Data.Username
        };

        await _userRepository.AddUserAsync(newUser.Data);

        var identityResult = await _userManager.CreateAsync(appUser, password);

        if (!identityResult.Succeeded)
        {
            var errorMessage = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return Result<Guid>.Failure(new Error("Identity.RegistrationFailed", errorMessage));
        }

        return Result<Guid>.Success(appUser.DomainUserId);
    }
}
