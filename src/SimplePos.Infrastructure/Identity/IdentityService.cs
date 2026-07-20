using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    public Task<Result<string>> LoginAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Guid>> RegisterUserAsync(string username, string email, string password, Guid outletId)
    {
        throw new NotImplementedException();
    }
}
