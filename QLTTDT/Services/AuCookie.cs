using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.VisualBasic;
using QLTTDT.Data;

namespace QLTTDT.Services
{
    public class AuCookie
    {
        public static ClaimsPrincipal CreatePrincipal(int id, string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, role + "Identity");
            return new ClaimsPrincipal(identity);
        }
        public static AuthenticationProperties CreateProperties()
        {
            return new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(ConstantValues.COOKIE_EXPIRY_DAYS),
            };
        }
    }
}
