using System.Security.Principal;

namespace int3306
{
    public class IdentityUtilities
    {
        public static GenericIdentity ConstructIdentity(int userId) => new(userId.ToString(), "user");
    }
}