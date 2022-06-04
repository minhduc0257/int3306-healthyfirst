using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace int3306.Controllers
{
    [Authorize]
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ExtendedController
    {
        public AuthenticationController(DataDbContext dbContext) : base(dbContext) {}

        /// <summary>
        /// Log in.
        /// </summary>
        /// <param name="user">An user object.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<OAuth2TokenObject>> Login([FromBody] User user)
        {
            var authenticatedUser = await DBContext.Users
                .Where(u => u.Username == user.Username)
                .FirstOrDefaultAsync();

            if (authenticatedUser == null) return Unauthorized();
            if (!BCrypt.Net.BCrypt.Verify(user.Password, authenticatedUser.Password))
            {
                return Unauthorized();
            }

            var identity = IdentityUtilities.ConstructIdentity(authenticatedUser);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            claimsPrincipal.AddIdentity(new ClaimsIdentity());
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true
                }
            );
            
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = identity,
                Issuer = Keys.TokenValidationParameters.ValidIssuer,
                Audience = Keys.TokenValidationParameters.ValidAudience,
                Expires = DateTime.UtcNow + TimeSpan.FromDays(1),
                SigningCredentials = new SigningCredentials(Keys.SigningKey, SecurityAlgorithms.HmacSha256Signature)
            });

            return new OAuth2TokenObject
            {
                Token = handler.WriteToken(token)
            };
        }
        
        /// <summary>
        /// Get information of the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("whoami")]
        public async Task<ActionResult<User>> WhoAmI()
        {
            var id = User.GetUserId();
            if (id == null) return NotFound();
            var user = await DBContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            user.Password = null!;
            return user;
        }

        /// <summary>
        /// Log out.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}