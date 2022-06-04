using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [ApiController]
    [Authorize]
    [Route("users")]
    public class UserController : ExtendedController
    {
        public UserController(DataDbContext dbContext) : base(dbContext) {}

        /// <summary>
        /// List all users. 401 if you're not an admin.
        /// </summary>
        /// <returns>List of all users.</returns>
        [HttpGet]
        public async Task<ActionResult<User[]>> ListUsers()
        {
            if (!await IsPrivileged())
            {
                return Unauthorized("you are a normal user");
            }
            
            var res = await DBContext.Users.ToArrayAsync();
            foreach (var u in res)
                u.Password = null!;

            return res;
        }

        /// <summary>
        /// Create a normal user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (!await IsPrivileged())
            {
                return Unauthorized("you are a normal user");
            }

            if (await DBContext.Users.AnyAsync(u => u.Username == user.Username.ToLowerInvariant()))
            {
                return Conflict($"an user with username \"{user.Username.ToLowerInvariant()}\" already exists");
            }

            var newUser = new User
            {
                Username = user.Username.ToLowerInvariant(),
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Type = UserType.User
            };

            var res = DBContext.Add(newUser);
            res.Entity.Password = null!;
            await DBContext.SaveChangesAsync();
            return res.Entity;
        }
        
        /// <summary>
        /// Delete a normal user.
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> CreateUser(int id)
        {
            if (!await IsPrivileged())
            {
                return Unauthorized("you are a normal user");
            }

            var user = await DBContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            if (user.Type == UserType.Admin)
            {
                return Unauthorized("you can't delete an admin!");
            }
            
            DBContext.Users.Remove(user);
            await DBContext.SaveChangesAsync();
            return Ok();
        }
    }
}