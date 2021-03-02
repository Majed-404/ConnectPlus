using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(AppDbContext context,
            ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto _registerDto)
        {
            if(await UserExists(_registerDto.UserName))
            {
                return BadRequest("Username is taken.");
            }

            using var _hmac = new HMACSHA512();
            var _user = new AppUser
            {
                UserName = _registerDto.UserName.ToLower(),
                PasswordHash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(_registerDto.Password)),
                PasswordSalt = _hmac.Key
            };

            _context.Users.Add(_user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = _user.UserName,
                Token = _tokenService.CreateToken(_user)
            };
        }

        private async Task<bool> UserExists(string _userName)
        {
            return await _context.Users.AnyAsync(ECKeyXmlFormat => ECKeyXmlFormat.UserName == _userName.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto _loginDto)
        {
            var _user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _loginDto.UserName);

            if(_user == null)
            {
                return Unauthorized("Invalid username.");
            }

            using var _hmac = new HMACSHA512(_user.PasswordSalt);

            var _computedHash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(_loginDto.Password));

            for(int i=0; i < _computedHash.Length; i++)
            {
                if(_computedHash[i] != _user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password.");
                }
            }

            return new UserDto
            {
                UserName = _user.UserName,
                Token = _tokenService.CreateToken(_user)
            };
        }
    }
}
