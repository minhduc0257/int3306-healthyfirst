using Microsoft.AspNetCore.Mvc;

namespace int3306.Controllers
{
    [ApiController]
    [Route(Route)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UnauthorizedController : Controller
    {
        public const string Route = "authfail";
        
        public IActionResult Get()
        {
            return Unauthorized("you are not an admin");
        }
    }
}