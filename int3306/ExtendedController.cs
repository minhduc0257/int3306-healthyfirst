using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306
{
    [ApiController]
    public class ExtendedController : Controller
    {
        protected readonly DataDbContext DBContext;
        public ExtendedController(DataDbContext dbContext) => DBContext = dbContext;

        protected Task<bool> IsPrivileged() => IsAdmin(DBContext, User);

        public static async Task<bool> IsAdmin(DataDbContext dbContext, ClaimsPrincipal claimsPrincipal)
        {
            var uid = claimsPrincipal.GetUserId();
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == uid);
            return user?.Type == UserType.Admin;
        }
    }
}