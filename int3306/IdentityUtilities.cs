using System.Security.Claims;
using System.Security.Principal;

namespace int3306
{
    public class IdentityUtilities
    {
        public static ClaimsIdentity ConstructIdentity(User user) => new ClaimsIdentity(
            new GenericIdentity(user.Id.ToString(), "user"),
            new[]
            {
                new Claim("type", user.Type.ToString())
            }
        );
    }
}