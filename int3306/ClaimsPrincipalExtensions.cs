using System.Security.Claims;

namespace int3306
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal principal)
        {
            var name = principal.Identity?.Name;
            return name != null ? int.Parse(name) : null;
        }
    }
}