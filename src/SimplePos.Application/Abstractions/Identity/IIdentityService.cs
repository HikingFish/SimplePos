using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Abstractions.Identity;
public interface IIdentityService
{
    Task<Result<Guid>> RegisterUserAsync(string username, string email, string password, Guid companyId, Guid outletId, string phoneNumber, string userPosition);
    Task<Result<string>> LoginAsync(string email, string password);
}