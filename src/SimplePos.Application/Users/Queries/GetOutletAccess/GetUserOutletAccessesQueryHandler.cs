using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Users;

namespace SimplePos.Application.Users.Queries.GetOutletAccess;

public class GetUserOutletAccessesQueryHandler : IQueryHandler<GetUserOutletAccessesQuery, Result<List<UserOutletAccessResponse>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserOutletAccessRepository _userOutletAccessRepository;
    private readonly IOutletRepository _outletRepository;

    public GetUserOutletAccessesQueryHandler(
        IUserRepository userRepository,
        IUserOutletAccessRepository userOutletAccessRepository,
        IOutletRepository outletRepository)
    {
        _userRepository = userRepository;
        _userOutletAccessRepository = userOutletAccessRepository;
        _outletRepository = outletRepository;
    }

    public async Task<Result<List<UserOutletAccessResponse>>> HandleAsync(
        GetUserOutletAccessesQuery query, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(query.UserId);
        if (user == null)
        {
            return Result<List<UserOutletAccessResponse>>.Failure(UserError.AccountNotFound);
        }

        var accessList = await _userOutletAccessRepository.GetUserOutletAccessesByUserIdAsync(query.UserId);
        var response = new List<UserOutletAccessResponse>();

        foreach (var access in accessList)
        {
            var outlet = await _outletRepository.GetOutletByIdAsync(access.OutletId);
            if (outlet != null)
            {
                response.Add(new UserOutletAccessResponse(
                    outlet.OutletId,
                    outlet.Name,
                    outlet.PhoneNumber,
                    outlet.OutletAddress.City,
                    outlet.OutletAddress.State
                ));
            }
        }

        return Result<List<UserOutletAccessResponse>>.Success(response);
    }
}
