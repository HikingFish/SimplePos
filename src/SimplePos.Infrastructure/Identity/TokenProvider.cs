using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimplePos.Infrastructure.Identity
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IConfiguration _configuration;
        public TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(User user, IEnumerable<string> permissions)
        {
            var secretKey = _configuration["Jwt:Secret"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value),
                new("OutletId", user.OutletId.ToString())
            };

            throw new NotImplementedException();
        }
    }
}
