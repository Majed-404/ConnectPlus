using API.Data;
using API.DTOs;
using API.Entities;
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

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto _registerDto)
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

            return _user;
        }

        private async Task<bool> UserExists(string _userName)
        {
            return await _context.Users.AnyAsync(ECKeyXmlFormat => ECKeyXmlFormat.UserName == _userName.ToLower());
        }
    }
}
