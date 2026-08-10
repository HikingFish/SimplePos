using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Registrations.Commands.RegisterBusiness;

public class RegisterBusinessCommandHandler : ICommandHandler<RegisterBusinessCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICompanyRepository _companyRepository;
    private readonly IOutletRepository _outletRepository;
    private readonly IUserRepository _userRepository;

    public RegisterBusinessCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService, ICompanyRepository companyRepository, IOutletRepository outletRepository, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _companyRepository = companyRepository;
        _outletRepository = outletRepository;
        _userRepository = userRepository;
    }

    public async Task<Result> HandleAsync(RegisterBusinessCommand command, CancellationToken cancellationToken)
    {
        var companyAddress = Address.Create(command.Street, command.City, command.State, command.PostalCode, command.Country);
        var companyEmail = EmailAddress.Create(command.CompanyEmail);
        var companyResult = Company.Create(command.CompanyName, companyAddress.Data, command.CompanyPhoneNumber, companyEmail.Data);

        var outletAddress = Address.Create(command.Street, command.City, command.State, command.PostalCode, command.Country);
        var outletResult = Outlet.Create(companyResult.Data.CompanyId,command.CompanyName + "HQ", outletAddress.Data, command.CompanyPhoneNumber);

        var userEmailAddress = EmailAddress.Create(command.Email);
        var userResult = User.Create(outletResult.Data.OutletId, companyResult.Data.CompanyId, command.Username, userEmailAddress.Data, command.PhoneNumber, "Administrator");

        _companyRepository.AddCompany(companyResult.Data);
        _outletRepository.AddOutlet(outletResult.Data);
        _userRepository.AddUser(userResult.Data);

        // Save domain entities first so FK references exist in the DB
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        return Result.Success();
    }
}