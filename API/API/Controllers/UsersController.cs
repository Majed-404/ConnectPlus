using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var _users = await _context.Users.ToListAsync();

            return _users;
        }

        // api/users/2
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int _id)
        {
            var _user = await _context.Users.FindAsync(_id);

            return _user;
        }
    }
}
