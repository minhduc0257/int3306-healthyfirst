using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306
{
    [ApiController]
    public class ExtendedController : Controller
    {
        protected readonly DataDbContext DBContext;
        public ExtendedController(DataDbContext dbContext) => DBContext = dbContext;
        
        protected async Task<bool> IsPrivileged()
        {
            var uid = User.GetUserId();
            var user = await DBContext.Users.FirstOrDefaultAsync(user => user.Id == uid);
            return user?.Type == UserType.Admin;
        }
    }
}