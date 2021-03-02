using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
        }


        public string CreateToken(AppUser user)
        {
            var _claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            var _creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
          
            var _tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(_claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = _creds
            };

            var _tokenHandler = new JwtSecurityTokenHandler();

            var _token = _tokenHandler.CreateToken(_tokenDescriptor);

            return _tokenHandler.WriteToken(_token);
        }
    }
}
