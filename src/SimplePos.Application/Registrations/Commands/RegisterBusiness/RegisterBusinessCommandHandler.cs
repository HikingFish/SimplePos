using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Registrations.Commands.RegisterBusiness;

public class RegisterBusinessCommandHandler : ICommandHandler<RegisterBusinessCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICompanyRepository _companyRepository;
    private readonly IOutletRepository _outletRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RegisterBusinessCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService, ICompanyRepository companyRepository, IOutletRepository outletRepository, IUserRepository userRepository, IPermissionRepository permissionRepository)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _companyRepository = companyRepository;
        _outletRepository = outletRepository;
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<Result> HandleAsync(RegisterBusinessCommand command, CancellationToken cancellationToken)
    {
        var companyAddress = Address.Create(command.Street, command.City, command.State, command.PostalCode, command.Country);
        if (!companyAddress.IsSuccess || companyAddress.Data == null)
            return Result.Failure(companyAddress.Error);

        var companyEmail = EmailAddress.Create(command.CompanyEmail);
        if (!companyEmail.IsSuccess || companyEmail.Data == null)
            return Result.Failure(companyEmail.Error);

        var companyPhone = PhoneNumber.Create(command.CompanyPhoneNumber);
        if (!companyPhone.IsSuccess || companyPhone.Data == null)
            return Result.Failure(companyPhone.Error);

        var companyResult = Company.Create(command.CompanyName, companyAddress.Data, companyPhone.Data, companyEmail.Data);
        if (!companyResult.IsSuccess || companyResult.Data == null)
            return Result.Failure(companyResult.Error);

        var outletAddress = Address.Create(command.Street, command.City, command.State, command.PostalCode, command.Country);
        if (!outletAddress.IsSuccess || outletAddress.Data == null)
            return Result.Failure(outletAddress.Error);

        var outletResult = Outlet.Create(companyResult.Data.CompanyId, command.CompanyName + "HQ", outletAddress.Data, companyPhone.Data);
        if (!outletResult.IsSuccess || outletResult.Data == null)
            return Result.Failure(outletResult.Error);

        var userEmailAddress = EmailAddress.Create(command.Email);
        if (!userEmailAddress.IsSuccess || userEmailAddress.Data == null)
            return Result.Failure(userEmailAddress.Error);

        PhoneNumber? userPhone = null;
        if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
        {
            var userPhoneResult = PhoneNumber.Create(command.PhoneNumber);
            if (!userPhoneResult.IsSuccess || userPhoneResult.Data == null)
                return Result.Failure(userPhoneResult.Error);
            userPhone = userPhoneResult.Data;
        }

        var userResult = User.Create(outletResult.Data.OutletId, companyResult.Data.CompanyId, command.Username, userEmailAddress.Data, userPhone, "Administrator");
        if (!userResult.IsSuccess || userResult.Data == null)
            return Result.Failure(userResult.Error);

        var adminPermission = await _permissionRepository.GetPermissionByNameAsync("Admin");
        if (adminPermission == null)
            return Result.Failure(UserError.AdminPermissionNotFound);
        
        userResult.Data.AssignPermission(adminPermission.PermissionId);

        _companyRepository.AddCompany(companyResult.Data);
        _outletRepository.AddOutlet(outletResult.Data);
        _userRepository.AddUser(userResult.Data);
        

        // Now create the identity user (UserManager.CreateAsync calls SaveChangesAsync internally)
        var identityResult = await _identityService.RegisterUserAsync(
            userResult.Data.UserId,
            command.Username,
            userResult.Data.Email.Value,
            command.Password);

        if (!identityResult.IsSuccess)
        {
            return Result.Failure(identityResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}