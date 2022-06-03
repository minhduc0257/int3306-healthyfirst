using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [ApiController]
    [Authorize]
    [Route("users")]
    public class UserController : Controller
    {
        private readonly DataDbContext dbContext;
        public UserController(DataDbContext dbContext) => this.dbContext = dbContext;

        private async Task<bool> IsPrivileged()
        {
            var uid = User.GetUserId();
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == uid);
            return user?.Type == UserType.Admin;
        }
        
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
            
            var res = await dbContext.Users.ToArrayAsync();
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

            if (await dbContext.Users.AnyAsync(u => u.Username == user.Username.ToLowerInvariant()))
            {
                return Conflict($"an user with username \"{user.Username.ToLowerInvariant()}\" already exists");
            }

            var newUser = new User
            {
                Username = user.Username.ToLowerInvariant(),
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Type = UserType.User
            };

            var res = dbContext.Add(newUser);
            res.Entity.Password = null!;
            await dbContext.SaveChangesAsync();
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

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            if (user.Type == UserType.Admin)
            {
                return Unauthorized("you can't delete an admin!");
            }
            
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}