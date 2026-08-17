using SimplePos.Domain.Companies;
using SimplePos.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Abstractions.Identity;
public interface ITokenProvider
{
    string CreateToken(User user);
}
