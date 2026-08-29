using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SimplePos.Application.Auths.Queries;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOutletRepository _outletRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    public GetCurrentUserQueryHandler(
        IUserRepository userRepository,
        IOutletRepository outletRepository,
        ICompanyRepository companyRepository,
        IUserPermissionRepository userPermissionRepository)
    {
        _userRepository = userRepository;
        _outletRepository = outletRepository;
        _companyRepository = companyRepository;
        _userPermissionRepository = userPermissionRepository;
    }
    public async Task<Result<CurrentUserResponse>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        User? userResult = await _userRepository.GetUserByIdAsync(query.UserId);
        if(userResult == null)
            return Result<CurrentUserResponse>.Failure(UserError.AccountNotFound);
        
        Outlet? outletResult = null;
        if(userResult.OutletId is Guid outletId)
            outletResult = await _outletRepository.GetOutletByIdAsync(outletId);

        Company? companyResult = await _companyRepository.GetCompanyByIdAsync(userResult.CompanyId);

        if(companyResult == null)
            return Result<CurrentUserResponse>.Failure(CompanyError.CompanyNotFound);

        List<UserPermission> permissionsResult = await _userPermissionRepository.GetUserPermissionsByUserIdAsync(userResult.UserId);

        CurrentCompany currentCompany = new CurrentCompany(companyResult.Name, companyResult.Email.Value, companyResult.PhoneNumber);

        CurrentOutlet currentOutlet = null;
        if(outletResult != null)
            currentOutlet = new CurrentOutlet(outletResult.Name, outletResult?.PhoneNumber);

        CurrentUserResponse currentUserResponse = new CurrentUserResponse(userResult.Username, userResult.Email.Value, userResult.PhoneNumber, userResult.UserPosition, userResult.IsActive, currentCompany, currentOutlet, permissionsResult.Select(p => p.PermissionId.ToString()).ToList());
        return Result<CurrentUserResponse>.Success(currentUserResponse);
    }
}
