using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SellersZone.Infra.Helpers
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]));
        }

        public (string Token, long Expires) CreateToken(AppUser user, dynamic role)
        {
            //keep the information inside token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName + " " + user.LastName + " - " + user.StoreName),
                new Claim(ClaimTypes.Role, role[0])
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var expirationTime = DateTime.UtcNow.AddHours(8);

            var unixTimestamp = ((DateTimeOffset)expirationTime).ToUnixTimeMilliseconds();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                SigningCredentials = creds,
                Issuer = _configuration["Token:Issuer"] //Issuer => how creat and signd this token
            };          

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), unixTimestamp);
        }

    }
}
