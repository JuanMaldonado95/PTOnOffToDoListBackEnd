using ApplicationCore.Interfaces.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApiPTBackOnOff.Shared.Utils;

namespace Infrastructure.Services.Auth
{
    public class TokenGeneratorService : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public TokenGeneratorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateJwtSecurityToken(Dictionary<string, string> claimsDictionary)
        {
            try
            {
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, claimsDictionary.FirstOrDefault(c => c.Key == "id").Value),
                new Claim("idUser", claimsDictionary.FirstOrDefault(c => c.Key == "id").Value)
            };

                string userValue;
                if ((userValue = claimsDictionary.FirstOrDefault(c => c.Key == "user").Value) != null)
                    claims.Add(new Claim(JwtRegisteredClaimNames.Name, userValue));

                var jwtToken = GenerateToken(claims);

                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                return token;
            }
            catch (Exception ex)
            {
                GenericUtils.Log("TokenGeneratorService: Error en el servicio CreateJwtSecurityToken", ex);
                throw;
            }
        }

        private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                double timeExpire = 20;

                return new JwtSecurityToken(
                    _configuration["JWT:Issuer"],
                    _configuration["JWT:Issuer"],
                    claims,
                    expires: DateTime.Now.AddMinutes(timeExpire),
                    signingCredentials: credentials);
            }
            catch (Exception ex)
            {
                GenericUtils.Log("TokenGeneratorService: Error en el servicio GenerateToken", ex);
                throw;
            }
        }
    }
}
