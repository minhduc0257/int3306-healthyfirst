using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace int3306
{
    public class CookieAuthenticationEvents : Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
    {
        private readonly DataDbContext dbContext;
        public CookieAuthenticationEvents(DataDbContext dbContext) => this.dbContext = dbContext;
        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> ctx)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext ctx)
        {
            var id = ctx.Principal?.GetUserId();
            if (id == null)
            {
                ctx.RejectPrincipal();
                return;
            }

            var hasValid = await dbContext.Users.AnyAsync(u => u.Id == id);
            if (!hasValid)
            {
                ctx.RejectPrincipal();
            }
        }
    }
}